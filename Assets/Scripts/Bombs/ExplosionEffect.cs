using UnityEngine;
using V1king;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles = default;

    [SerializeField] private float maxForceToApply = 1f;
    [SerializeField] private LayerMask layersToPush = default;
    [SerializeField] private int playerLayer = default;

    private float radius;

    private void OnEnable()
    {
        Invoke(nameof(PhysicsPush), 0.01f);
        particles.Play();
        Invoke(nameof(Despawn), particles.main.duration);
    }

    private void PhysicsPush()
    {
        radius = transform.localScale.x * 3f;
        Collider2D[] results = new Collider2D[20];
        int hitAmount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, results, layersToPush);
        for(int i = 0; i < hitAmount; ++i)
        {
            Vector3 dir = (results[i].transform.position - transform.position).normalized;
            float dist = Vector3.Distance(results[i].transform.position, transform.position);
            float force = Mathf.Lerp(0f, maxForceToApply, Mathf.Sqrt((radius - dist) / radius));
            if(force > 0f)
            {
                results[i].attachedRigidbody.AddForce(dir * force, ForceMode2D.Impulse);
                if(results[i].attachedRigidbody.gameObject.layer == playerLayer)
                {
                    CameraController.singleton.Shake(force, 0.5f);
                    PPController.singleton.SetVignette(MathConversions.ConvertNumberRange(force, 0f, 20f, 0f, 0.65f));

                    // Add action score
                    Player player = results[i].attachedRigidbody.GetComponent<Player>();
                    ScoreManager.singleton.AddActionScore(player, force);
                }
            }
        }
    }

    private void Despawn()
    {
        ObjectPooler.singleton.Despawn(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
