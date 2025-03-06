using System.Collections;
using UnityEngine;

public class PowerupAnimation : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        const float duration = 1f;
        float rot = 1.25f;
        while(true)
        {
            float time = 0f;
            rot *= -1f;
            while(time < duration)
            {
                time += Time.deltaTime;
                transform.localScale = new Vector3(Mathf.SmoothStep(rot * -1f, rot, time / duration), Mathf.SmoothStep(1.25f, 1.5f, time / duration), 1f);
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0.25f, 0.5f, Mathf.Sin(Mathf.SmoothStep(0f, Mathf.PI, time / duration))), transform.localPosition.z);
                yield return null;
            }
        }
    }
}
