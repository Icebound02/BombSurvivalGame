using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    private static readonly WaitForSeconds enableLightDelay = new WaitForSeconds(1f);

    [SerializeField] private bool hasDelay = default;
    [SerializeField] private float duration = default;
    [SerializeField] private GameObject lights = default;

    private void Awake()
    {
        StartCoroutine(EnableLightFlicker(hasDelay, duration, lights));
    }

    public static IEnumerator EnableLightFlicker(bool hasDelay, float duration, GameObject lights)
    {
        if(hasDelay)
            yield return enableLightDelay;

        float endTime = Time.unscaledTime + duration;
        while(Time.unscaledTime < endTime)
        {
            lights.SetActive(!lights.activeSelf);
            yield return new WaitForSecondsRealtime(Mathf.Pow(Random.value, 3f) * 0.25f);
        }
        lights.SetActive(true);
    }
}
