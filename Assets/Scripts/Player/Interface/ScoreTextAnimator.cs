using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreTextAnimator : MonoBehaviour
{
    private static WaitForSecondsRealtime delay;

    public TextMeshProUGUI text;

    [SerializeField] private float scaleMultiplier = 2f;
    [SerializeField] private float animDuration = 1f;
    [SerializeField] private float animDelay = 1f;

    private Vector3 originalScale;
    private float amount;
    private int lastNumberDisplayed;

    private void Awake()
    {
        delay = new WaitForSecondsRealtime(animDelay);
        originalScale = transform.localScale;
    }

    public void StartAnimating(float amount, string scoreType)
    {
        this.amount += amount;

        int numberToDisplay = Mathf.RoundToInt(this.amount);
        if(numberToDisplay != lastNumberDisplayed && numberToDisplay != 0)
        {
            text.text = $"+{numberToDisplay} {scoreType}";
            gameObject.SetActive(true);
            StopCoroutine(Animate());
            StartCoroutine(Animate());
        }

        lastNumberDisplayed = numberToDisplay;
    }

    private IEnumerator Animate()
    {
        yield return delay;

        float time = 0f;
        while(time < animDuration)
        {
            time += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, time / animDuration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, (animDuration - time) / animDuration);
            yield return null;
        }

        gameObject.SetActive(false);
        transform.localScale = originalScale;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        lastNumberDisplayed = 0;
        amount = 0;
    }
}
