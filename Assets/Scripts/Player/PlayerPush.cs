using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [SerializeField] private Player player = default;

    [SerializeField] private float pushCooldown = default;
    private float nextPush = float.NegativeInfinity;

    [SerializeField] private Bounds bounds = default;
    [SerializeField] private LayerMask layers = default;
    [SerializeField] private Vector2 force = default;

    public bool isKicking = false;
    public float isKickingStartTimer = 0.1f;
    public float isKickingTimer = 0.1f;
    public Animator anim;

    [SerializeField] private AudioClip audioMiss = default;
    [SerializeField] private AudioClip audioHit = default;

    private void Update()
    {
        if(player.GetKeyDown(KeyMaps.Use))
            Push();

        if (isKicking) {
            anim.SetBool("IsKicking", true);
            isKickingTimer -= Time.deltaTime;
        }

        if (isKickingTimer <= 0) {
            anim.SetBool("IsKicking", false);
            isKicking = false;
            isKickingTimer = isKickingStartTimer;
        }
    }

    private bool CanPush()
    {
        return Time.time > nextPush && (!player.powerup.powerup || player.powerup.powerup.useKey != KeyMaps.Use);
    }

    private Vector3 GetCenter()
    {
        Vector3 center = transform.position + bounds.center;
        if(!player.movement.facingRight)
            center.x = transform.position.x - bounds.center.x;
        return center;
    }

    private Vector3 GetDirection()
    {
        return player.movement.facingRight ? Vector3.right : Vector3.left;
    }

    private void Push()
    {
        if (!CanPush())
            return;

        Collider2D[] results = new Collider2D[Player.MAX_PLAYERS - 1]; // Can't hit self
        int hitAmount = Physics2D.OverlapBoxNonAlloc(GetCenter(), bounds.size, 0f, results, layers);
        if(hitAmount > 0)
            player.audioSource.PlayOneShot(audioHit);
        else
            player.audioSource.PlayOneShot(audioMiss);
        for(int i = 0; i < hitAmount; ++i)
        {
            results[i].attachedRigidbody.AddForce(GetDirection() * force.x + Vector3.up * force.y, ForceMode2D.Impulse);
            results[i].attachedRigidbody.sharedMaterial = player.movement.noFriction;
            results[i].GetComponent<PlayerMovement>().lastPushed = Time.time;
        }
        if(hitAmount > 0)
            ObjectPooler.singleton.SpawnKickEffect(GetCenter());
        nextPush = Time.time + pushCooldown;

        isKicking = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetCenter(), bounds.size);
    }
#endif
}
