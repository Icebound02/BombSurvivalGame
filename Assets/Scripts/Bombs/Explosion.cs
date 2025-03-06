using UnityEngine;

public class Explosion : MonoBehaviour
{
    private const float MAX_EXPLOSION_SIZE = 10f;
    [SerializeField] private AudioClip audioExplosion = default;

    [Range(0f, MAX_EXPLOSION_SIZE)]
    [SerializeField] private float explosionSize = 2; // Explosion size in standard units
    protected int scaledExplosionSize; // Explosion size scaled accordingly to terrain size, only for use in terrain calculations

    [SerializeField] private LayerMask destroyableLayers = default;
    
    private void Start()
    {
        scaledExplosionSize = Mathf.RoundToInt(explosionSize * TerrainManager.singleton.PPU);
    }

    public virtual void Explode()
    {
        // Explosion audio & effects
        // This uses AudioManager, since the object is destroyed. AudioSource is therefore on external objects
        AudioManager.PlayAudioAt(audioExplosion, transform.position, Mathf.Lerp(3f, 0.2f, scaledExplosionSize / MAX_EXPLOSION_SIZE) + Random.Range(-0.1f, 0.1f));
       
        ObjectPooler.singleton.SpawnExplosion(transform.position, Vector3.one * explosionSize);
        
        // Terrain deformation
        TerrainManager.singleton.Explode(transform.position, scaledExplosionSize);

        // Kill players
        Collider2D[] hitColliders = new Collider2D[10];
        int hitAmount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionSize + Player.halfWidth, hitColliders, destroyableLayers);
        for(int i = 0; i < hitAmount; ++i)
        {
            // Kill players
            Player player = hitColliders[i].GetComponent<Player>();
            if(player)
                hitColliders[i].GetComponent<Player>().Die();
            else
            {
                // Destroy carryables
                /*Carryable carryable = hitColliders[i].GetComponent<Carryable>();
                if(carryable)
                    Destroy(hitColliders[i].gameObject);*/

                // Destroy powerups
                Powerup powerup = hitColliders[i].GetComponent<Powerup>();
                if(powerup)
                    Destroy(hitColliders[i].gameObject);
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw explosion size in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionSize);
        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionSize + Player.halfWidth);
        }
    }
#endif
}
