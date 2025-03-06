using UnityEngine;
using V1king;

public class GraveAnimator : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float scaleMultiplier = 1.25f;

    private void Update()
    {
        float animValue = 1f + Mathf.Sin(Time.time * speedMultiplier);
        transform.localScale = Vector3.one * MathConversions.ConvertNumberRange(animValue, 0f, 2f, 1f / scaleMultiplier, 1f * scaleMultiplier);
    }
}
