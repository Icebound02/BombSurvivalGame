using UnityEngine;

public class PowerForceField : Powerup
{
    private const float RADIUS = 5f;

    [SerializeField] private GameObject prefabEffect = default;
    [SerializeField] private float radiusOffset = default;

    [SerializeField] private float maxForceToApply = 10f;
    [SerializeField] private LayerMask layersToPush = default;

    [SerializeField] private int uses = 1;
    private float usageTimeIncrease;

    private void Awake()
    {
        usageTimeIncrease = maxUsageTime / uses + 0.001f;
    }

    protected override void GetPowerup()
    {
        useKey = KeyMaps.Use;
        use.AddListener(UsePowerup);
        useOnce = true;

        base.GetPowerup();
    }

    protected override void UsePowerup()
    {
        GameObject newObj = Instantiate(prefabEffect, player.transform.position, Quaternion.identity, null);
        newObj.transform.localScale = Vector3.one * RADIUS * radiusOffset;
        PhysicsPush();

        usageTime += usageTimeIncrease;
    }

    private void PhysicsPush()
    {
        Collider2D[] results = new Collider2D[20];
        int hitAmount = Physics2D.OverlapCircleNonAlloc(player.transform.position, RADIUS, results, layersToPush);
        for(int i = 0; i < hitAmount; ++i)
        {
            if(results[i].gameObject == player.gameObject) // Don't push self
                continue;
            Vector3 dir = (results[i].transform.position - player.transform.position).normalized;
            float dist = Vector3.Distance(results[i].transform.position, player.transform.position);
            float force = Mathf.Lerp(0f, maxForceToApply, Mathf.Sqrt((RADIUS - dist) / RADIUS));
            if(force > 0f)
                results[i].attachedRigidbody.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!player)
            return;

        Gizmos.DrawWireSphere(player.transform.position, RADIUS);
    }
#endif
}
