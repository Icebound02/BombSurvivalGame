using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPController : MonoBehaviour
{
    public static PPController singleton;

    [SerializeField] private VolumeProfile postProcess = null;
    private Vignette vignette;

    [SerializeField] private float vignetteDegradationSpeed = 1f;

    private void Awake()
    {
        singleton = this;
        postProcess.TryGet(out vignette);
        SetVignette(0f);
    }

    private void Update()
    {
        vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, 0f, Time.deltaTime * vignetteDegradationSpeed);
    }

    public void SetVignette(float value)
    {
        vignette.intensity.value = value;
    }
}