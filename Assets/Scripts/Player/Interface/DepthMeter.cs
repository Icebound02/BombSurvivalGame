using UnityEngine;
using V1king;

public class DepthMeter : MonoBehaviour
{
    public static DepthMeter singleton;

    [SerializeField] private Transform fillMeter = default;
    [SerializeField] private float topAltitude = default;
    [SerializeField] private float bottomAltitude = default;

    private void Awake()
    {
        singleton = this;
    }

    public void SetAltitude(float altitude)
    {
        float normalizedValue = Mathf.Clamp01(MathConversions.ConvertNumberRange(altitude, bottomAltitude, topAltitude, 0f, 1f));
        fillMeter.localScale = new Vector3(fillMeter.localScale.x, normalizedValue, fillMeter.localScale.z);
    }
}
