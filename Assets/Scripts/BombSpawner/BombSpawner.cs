using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class BombSpawner : MonoBehaviour
{
    private static readonly WaitForSeconds blinkDuration = new WaitForSeconds(0.25f);

    [Header("Values")]
    [Tooltip("One movement, from one side to the other, in seconds")]
    [SerializeField] private float cycleDuration = 1f;
    [Tooltip("Specified in degrees")]
    [Range(0f, 90f)]
    [SerializeField] private float maxRotation = 75f;
    [Tooltip("Specified in seconds")]
    [SerializeField] private float minTimeBetweenSpawn = 0f;
    [Tooltip("Specified in seconds")]
    [SerializeField] private float maxTimeBetweenSpawn = 100f;
    [SerializeField] private GameObject[] prefabBombs = default;
    [SerializeField] private GameObject[] prefabPowerups = default;
    [SerializeField] private float bombThrowSpeed = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float chanceToSpawnPowerup = 0.5f;

    [SerializeField] private float introDifficultyDuration = 120f;

    [Header("Behaviours")]
    [SerializeField] private float minTimeBetweenBehaviourSwitch = 3f;
    [SerializeField] private float maxTimeBetweenBehaviourSwitch = 10f;
    [SerializeField] private float noiseScale = 1f;
    [SerializeField] private float noiseTravelSpeed = 1f;
    [SerializeField] private float minNoiseSpawnTime = 0f;
    [SerializeField] private float noiseSpawnTimeMultiplier = 1f;

    [Header("References")]
    [Tooltip("X scale specifies the extents, that this head moves along")]
    [SerializeField] private Transform rail = default;
    [SerializeField] private SpriteRenderer spriteRenderer = default;
    [SerializeField] private Transform rotAnchor = default;
    [SerializeField] private Transform body = default;
    [SerializeField] private Light2D rotateLight = default;
    [SerializeField] private Light2D moveLight = default;
    [SerializeField] private Light2D spotLight = default;
    [SerializeField] private AudioSource audioSource = default;

    private float extents;

    private Vector2 noisePos;

    // Behaviours
    private bool rotateFast;
    private bool moveFast;
    // Noise uses spotlight
    private bool useNoise; // For spawning bombs
    //private bool aimAtPlayer;

    private bool movePaused;

    private const float START_SPAWN_DIVISOR = 0.35f;
    private float bombSpawnDivisor = START_SPAWN_DIVISOR;

    private float lastBombSpawn = 0f;
    private float timeUntilNextSpawn;

    private void Awake()
    {
        noisePos = new Vector2(-9999f, GetRandomFloat()); // Random seed
        extents = rail.localScale.x / 2f - spriteRenderer.sprite.bounds.extents.x;
        StartCoroutine(Move());
        StartCoroutine(Rotate());
        UpdateNextBombSpawnTime();
        SwitchBehaviourInRandomTime();

        StartCoroutine(IntroDifficulty());
    }

    private void Update()
    {
        if(useNoise)
            spotLight.intensity = GetCurrentPerlinValue() * 2f;
        else
            spotLight.intensity = 0.75f;

        float highestDifficultyModifier = 1f;
        for(int i = 0; i < Player.players.Count; ++i)
        {
            Vector3 dirToPlayer = (Player.players[i].transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(-transform.up, dirToPlayer);

            if (dotProduct > 0f)
            {
                float difficultyModifier = 1f + dotProduct * Player.players[i].adaptiveDifficulty.difficultyIncrease;
                if(difficultyModifier > highestDifficultyModifier)
                    highestDifficultyModifier = difficultyModifier;
            }
            
        }
        
        //Debug.Log("Difficulty: " + difficultySpawnModifier + " | " + (timeUntilNextSpawn / difficultySpawnModifier));
        //Debug.Log("Time: " + Time.time + " | " + (lastBombSpawn + timeUntilNextSpawn / difficultySpawnModifier));
        if(Time.time >= lastBombSpawn + timeUntilNextSpawn / highestDifficultyModifier)
            SpawnBomb();
    }

    private float GetCurrentPerlinValue()
    {
        float xCoord = noisePos.x * noiseScale;
        float yCoord = noisePos.y * noiseScale;
        return Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord), minNoiseSpawnTime, 1f) * noiseSpawnTimeMultiplier;
    }

    private void ResetBehaviour()
    {
        rotateFast = false;
        moveFast = false;
        useNoise = false;

        StartCoroutine(BehaviourChangeIndicators());
        SwitchBehaviourInRandomTime();
    }
    private void ResetBehaviourInRandomTime()
    {
        Invoke(nameof(ResetBehaviour), Random.Range(minTimeBetweenBehaviourSwitch, maxTimeBetweenBehaviourSwitch) / 1.5f);
    }
    private void SwitchBehaviour()
    {
        rotateFast = GetRandomBool();
        moveFast = GetRandomBool();
        useNoise = GetRandomBool();
        /*aimAtPlayer = true;
        if(aimAtPlayer)
        {
            StopCoroutine(Rotate());
        }
        else
        {
            StartCoroutine(Rotate());
        }*/
        //Debug.Log($"rotateFast:{rotateFast} | moveFast:{moveFast} | useNoise:{useNoise} | aimAtPlayer:tbd");

        audioSource.Play();

        StartCoroutine(BehaviourChangeIndicators());
        ResetBehaviourInRandomTime();
    }
    private void SwitchBehaviourInRandomTime()
    {
        Invoke(nameof(SwitchBehaviour), Random.Range(minTimeBetweenBehaviourSwitch, maxTimeBetweenBehaviourSwitch));
    }

    private static float GetRandomFloat()
    {
        return Random.Range(-9999f, 9999f);
    }
    private static bool GetRandomBool()
    {
        return Random.value < 0.5f;
    }

    private void SpawnBomb()
    {
        lastBombSpawn = Time.time;

        GameObject prefabToSpawn = Random.value < chanceToSpawnPowerup ? prefabPowerups[Random.Range(0, prefabPowerups.Length)] : prefabBombs[Random.Range(0, prefabBombs.Length)];
        GameObject obj = Instantiate(prefabToSpawn, transform.position, Quaternion.identity, null);
        obj.GetComponent<Rigidbody2D>().velocity = -transform.up * bombThrowSpeed;
        StopCoroutine(FlashSpotlight());
        StartCoroutine(FlashSpotlight());
        UpdateNextBombSpawnTime();
    }
    private void UpdateNextBombSpawnTime()
    {
        if(useNoise)
        {
            float currentPerlinValue = GetCurrentPerlinValue();
            timeUntilNextSpawn = currentPerlinValue / bombSpawnDivisor;
            noisePos.x += noiseTravelSpeed;
            if(noisePos.x >= float.MaxValue)
                noisePos.x = -10000f;
        }
        else
            timeUntilNextSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn) / bombSpawnDivisor;
    }

    private IEnumerator FlashSpotlight()
    {
        const float duration = 0.5f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            spotLight.intensity = Mathf.SmoothStep(5f, 0.75f, time / duration);
            yield return null;
        }
    }

    private IEnumerator Move()
    {
        float fromPos = -extents;
        float toPos = extents;
        while(true)
        {
            float duration = cycleDuration;
            if(moveFast)
                duration /= 2f;

            float time = 0f;
            fromPos *= -1f;
            toPos *= -1f;
            while(time < duration)
            {
                if(!movePaused)
                {
                    time += Time.deltaTime;
                    body.localPosition = new Vector2(Mathf.SmoothStep(fromPos, toPos, time / duration), body.localPosition.y);
                }
                yield return null;
            }
        }
    }

    private IEnumerator Rotate()
    {
        float fromRot = -maxRotation;
        float toRot = maxRotation;

        while(true)
        {
            float rotDuration = cycleDuration;
            if(rotateFast)
                rotDuration /= 2f;
            else
                rotDuration *= 2f;

            fromRot *= -1f;
            toRot *= -1f;

            float time = 0f;
            while(time < rotDuration)
            {
                if(!movePaused)
                {
                    time += Time.deltaTime;
                    rotAnchor.localEulerAngles = new Vector3(0f, 0f, Mathf.SmoothStep(fromRot, toRot, time / rotDuration));
                }
                yield return null;
            }
        }
    }

    private IEnumerator BehaviourChangeIndicators()
    {
        movePaused = true;

        rotateLight.enabled = true;
        moveLight.enabled = true;
        spotLight.enabled = true;
        for(int i = 0; i < 8; ++i)
        {
            rotateLight.enabled = !rotateLight.enabled;
            moveLight.enabled = !moveLight.enabled;
            spotLight.enabled = !spotLight.enabled;
            yield return blinkDuration;
        }
        rotateLight.enabled = rotateFast;
        moveLight.enabled = moveFast;
        spotLight.color = useNoise ? Color.red : Color.white;
        yield return blinkDuration;

        movePaused = false;
    }

    private IEnumerator IntroDifficulty()
    {
        float time = 0f;
        while(time < introDifficultyDuration)
        {
            time += Time.deltaTime;
            bombSpawnDivisor = Mathf.Lerp(START_SPAWN_DIVISOR, 1f, time / introDifficultyDuration);
            yield return null;
        }
        Debug.Log("Intro difficulty finished");
    }
}
