using UnityEngine;
using V1king;
using TMPro;

/// <summary>
/// Calls for more bombs thrown at the player if he doesn't move much
/// </summary>
public class PlayerAdaptiveDifficulty : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI debugText = default;
    [Header("Values")]
    [SerializeField] private float recentBoundsDecaySpeed = 1f;
    [SerializeField] private float averageBoundsVolatility = 1f;
    [Tooltip("At what minimum magnitude of the average bounds should the difficulty start increasing")]
    [SerializeField] private float tooSmallBoundsMagnitude = 2f;

    [SerializeField] private float maxDifficultyMultiplier = 1f;

    /// <summary>
    /// Drawn as CYAN gizmos<br/>
    /// Average of where the player has been recently, expands bounds when player exits the area
    /// </summary>
    private Bounds recentBounds;
    /// <summary>
    /// Drawn as BLUE gizmos<br/>
    /// Gradually moves towards recent bounds<br/>
    /// This means, that if the player moves out of the average bounds shortly and comes back, it doesn't make much of a difference
    /// </summary>
    private Bounds averageBounds;

    /// <summary>
    /// Scaled value determining the amount to increase difficulty by<br/>
    /// Thereby a bomb spawn modifier
    /// </summary>
    [System.NonSerialized] public float difficultyIncrease;

    private void Awake()
    {
        recentBounds.center = transform.position;
        averageBounds.center = transform.position;
    }

    private void LateUpdate()
    {
        UpdateRecentBounds();
        UpdateAverageBounds();
        difficultyIncrease = Mathf.Clamp(MathConversions.ConvertNumberRange(averageBounds.size.magnitude, tooSmallBoundsMagnitude, 0f, 0f, 1f), 0f, maxDifficultyMultiplier);
        //Debug.Log(averageBounds.size.magnitude + " | " + difficultyIncrease);
        debugText.text = "Difficulty: " + Mathf.RoundToInt(difficultyIncrease * 100f) + "%";
    }

    private void UpdateRecentBounds()
    {
        float decayedX = Mathf.Lerp(recentBounds.size.x, 0f, Time.deltaTime * recentBoundsDecaySpeed);
        float decayedY = Mathf.Lerp(recentBounds.size.y, 0f, Time.deltaTime * recentBoundsDecaySpeed);
        recentBounds.size = new Vector3(decayedX, decayedY);

        // Horizontal
        if(transform.position.x > recentBounds.max.x)
            recentBounds.max = new Vector3(transform.position.x, recentBounds.max.y);
        else if(transform.position.x < recentBounds.min.x)
            recentBounds.min = new Vector3(transform.position.x, recentBounds.min.y);

        // Vertical
        if(transform.position.y > recentBounds.max.y)
            recentBounds.max = new Vector3(recentBounds.max.x, transform.position.y);
        else if(transform.position.y < recentBounds.min.y)
            recentBounds.min = new Vector3(recentBounds.min.x, transform.position.y);
    }

    private void UpdateAverageBounds()
    {
        averageBounds.max = Vector3.Lerp(averageBounds.max, recentBounds.max, Time.deltaTime * averageBoundsVolatility);
        averageBounds.min = Vector3.Lerp(averageBounds.min, recentBounds.min, Time.deltaTime * averageBoundsVolatility);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(recentBounds.center, recentBounds.size);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(averageBounds.center, averageBounds.size);
    }
#endif
}
