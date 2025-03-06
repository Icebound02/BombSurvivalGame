using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V1king;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player = default;

    [Header("Movement")]
    public float speed;
    public bool facingRight { get; private set; } = true;
    public Vector2 playerInput;

    [Header("Jumping")]
    public float jumpVelocity;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    private float distToGround;
    public float grav;
    public bool isJumping;
    [Header("Walljump")]
    [SerializeField] private float walljumpHorizontalForce = 1f;
    [SerializeField] private int maxConsecutiveWalljumps = 2;

    [Header("Components")]
    public Rigidbody2D rb;
    public Collider2D col;
    public LayerMask layers;
    public LayerMask headLayer;
    public Animator anim;

    [Header("Spotlight")]
    [SerializeField] private Transform spotlight = default;
    [SerializeField] private float spotlightRotationSpeed = 1f;

    private Vector3 originalScale;
    private float originalDrag;

    [Header("Slopes")]
    CapsuleCollider2D cc;
    Vector2 colliderSize;
    Vector2 slopeNormalPerp;
    public float slopeCheckDistance;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    public bool isOnSlope;
    private float slopeSideAngle;
    public float maxSlopeAngle;
    private bool canWalkOnSlope;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;
    public float yVelHelp;

    [Header("Push")]
    [SerializeField] private float pushDuration = 1f;
    public float lastPushed { private get; set; } = float.NegativeInfinity;

    [Space]
    [SerializeField] private Canvas debugCanvas = default;

    [System.NonSerialized] public List<Sticky> attachedBombs = new List<Sticky>();

    private int consecutiveWalljumps;

    [Header("Animations")]
    public float headBounceStartTime;
    public float headBounceTime;
    public float headBounceStartCooldown;
    public float headBounceCooldown;
    public bool CanBeJumpedOn = true;
    private bool IsJumpedOn = false;

    void Awake()
    {
        originalScale = transform.localScale;
        originalDrag = rb.drag;

        //Sætter variabler
        distToGround = col.bounds.extents.y;
        cc = GetComponent<CapsuleCollider2D>();
        colliderSize = cc.size;

        speed = 0.5f * transform.localScale.y;
        fallMultiplier = 3.8f * transform.localScale.y;
        lowJumpMultiplier = 1.3f * transform.localScale.y;
        jumpVelocity = 8.7f * transform.localScale.y;
        grav = 7.6f;
        slopeCheckDistance = 0.8f;
        maxSlopeAngle = 100;
        yVelHelp = 0.2f;
        CanBeJumpedOn = true;
        headBounceStartTime = 0.64f;
        headBounceTime = 0.64f;
        headBounceStartCooldown = 2f;
        headBounceCooldown = 2f;
    }

    // Start is called before the first frame update
    void Start()
    {

}

    void Update()
    {
        Vector2 vel = rb.velocity;

        bool isGrounded = IsGrounded();
        bool isWallsliding = IsPlayerWallSliding();

        if(isGrounded && rb.velocity.y <= 0f)
            consecutiveWalljumps = 0;
        
        //Animationer
        if((isGrounded || isWallsliding) && vel.y < 0f)
        {
            anim.SetBool("isJumping", false);
            anim.SetFloat("yVel", -1);
        }

        if (IsGrounded() && IsPlayerTouchingWall()) {
            anim.SetBool("OnGroundAndWall", true);
        } else {
            anim.SetBool("OnGroundAndWall", false);
        }

        if (IsGrounded()) {
            anim.SetBool("IsGrounded", true);
        } else {
            anim.SetBool("IsGrounded", false);
        }

        if (IsPlayerTouchingWall()) {
            anim.SetBool("IsOnWall", true);
        } else {
            anim.SetBool("IsOnWall", false);
        }

        if (IsJumpedOn) {
            anim.SetBool("IsJumpedOn", true);
            headBounceStartTime -= Time.deltaTime;
            CanBeJumpedOn = false;
        }

        if (headBounceStartTime <= 0) {
            anim.SetBool("IsJumpedOn", false);
            headBounceStartTime = headBounceTime;
            IsJumpedOn = false;
        }

        if (!CanBeJumpedOn) {
            headBounceStartCooldown -= Time.deltaTime;
        }

        if(headBounceStartCooldown <= 0) {
            CanBeJumpedOn = true;
            headBounceStartCooldown = headBounceCooldown;
        }


        //Jumping
        if (player.GetKeyDown(KeyMaps.Jump))
        {
            Invoke(nameof(ThrowStickyBombsOff), 0.1f);
            if(isGrounded || (isWallsliding && consecutiveWalljumps < maxConsecutiveWalljumps))
            {
                vel += Vector2.up * jumpVelocity;
                isJumping = true;
                if(isWallsliding)
                {
                    vel += facingRight ? Vector2.left * walljumpHorizontalForce * transform.localScale.y : Vector2.right * walljumpHorizontalForce * transform.localScale.y; // Walljump to opposite side
                    ++consecutiveWalljumps;
                }
                //StartCoroutine(JumpSqueeze(transform, originalScale, 0.8f, 1.4f, 0.1f, false));
                //anim.SetFloat("yVel", 1 * Mathf.Sign(vel.y));
            }
            StartCoroutine(JumpSqueeze(transform, originalScale, 0.8f, 1.4f, 0.1f, false));
            anim.SetBool("isJumping", true);
            anim.SetFloat("yVel", 1 * Mathf.Sign(vel.y));
        }

        if (rb.velocity.y <= 0.0f) {
            isJumping = false;
        }

        //Hop hurtigere opad
        if(IsGrounded() == false)
        {
            vel.y -= grav * Time.deltaTime;
        }
        rb.velocity = vel;

        //fald hurtigere end hop
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !player.GetKey(KeyMaps.Jump))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Wall interactions (Walljump/Wallslide)
        if(IsPlayerWallSliding())
            rb.drag = originalDrag * 10f;
        else
            rb.drag = originalDrag;
    }

    private void LateUpdate()
    {
        float desiredAngle;
        if(rb.velocity.sqrMagnitude > 5f)
            desiredAngle = VectorConversions.GetAngleFromVector(rb.velocity);
        else
            desiredAngle = facingRight ? 0f : 180f;
        float newAngle = Mathf.LerpAngle(spotlight.localEulerAngles.z, desiredAngle, Time.deltaTime * spotlightRotationSpeed);
        spotlight.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Movement
        SlopeCheck();
        ApplyMovement();

        //Vend spilleren den vej han går
        if((playerInput.x > 0 && !facingRight) || (playerInput.x < 0 && facingRight))
        {
            Flip();
        }
    }

    private bool IsPushed()
    {
        return Time.time <= lastPushed + pushDuration;
    }

    private void ApplyMovement()
    {
        playerInput = new Vector2(player.GetAxis(), 0);
        anim.SetFloat("xVel", Mathf.Abs(playerInput.x));

        if (IsGrounded() && !isOnSlope && !isJumping) {
            rb.AddForce(playerInput * speed, ForceMode2D.Impulse);
        } else if (isOnSlope && IsGrounded() && !isJumping && canWalkOnSlope) {
            rb.AddForce(new Vector2(speed * slopeNormalPerp.x * -playerInput.x, speed * slopeNormalPerp.y * -playerInput.x) + (Vector2)transform.up * yVelHelp, ForceMode2D.Impulse);
        } else if (!IsGrounded()) {
            rb.AddForce(playerInput * speed, ForceMode2D.Impulse);
        }

        if(player.powerup.powerup && player.powerup.powerup.toRotate)
        {
            if(playerInput.x > 0f || playerInput.x < 0f)
            {
                player.powerup.powerupRenderer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                if(player.GetKey(KeyMaps.Jump))
                    player.powerup.powerupRenderer.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
            }
            else if(player.GetKey(KeyMaps.Jump))
                player.powerup.powerupRenderer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            else
                player.powerup.powerupRenderer.transform.localEulerAngles = new Vector3(0f, 0, -90f);
        }
        else
            player.powerup.powerupRenderer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        debugCanvas.transform.localRotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        //spotlight.rotation = Quaternion.Euler(0, 0, facingRight ? 0 : 180);
    }

    private static IEnumerator JumpSqueeze(Transform transform, Vector3 originalScale, float xSqueeze, float ySqueeze, float duration, bool toRepeat)
    {
        //lav en lille squeeze når spilleren hopper
        Vector3 newSize = new Vector3(originalScale.x * xSqueeze, originalScale.y * ySqueeze, originalScale.z);
        float t = 0f;
        while(toRepeat)
        {
            while(t <= duration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, newSize, t / duration);
                yield return null;
            }
            t = 0f;
            while(t <= duration)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(newSize, originalScale, t / duration);

                yield return null;
            }
        }
    }

    private void ThrowStickyBombsOff()
    {
        // Sticky bombs fall off
        for(int i = 0; i < attachedBombs.Count; ++i)
        {
            for(int j = attachedBombs[i].joints.Count - 1; j >= 0; --j)
            {
                if(attachedBombs[i].joints[j].connectedBody == rb) // Destroy attached bombs
                {
                    Destroy(attachedBombs[i].joints[j], 0.1f);
                    attachedBombs[i].joints.RemoveAt(j);
                }
            }

            Sticky stickyBomb = attachedBombs[i].GetComponent<Sticky>();
            stickyBomb.StartCoroutine(stickyBomb.TimerForStickAgain());
            // attachedBombs[i].joint.connectedBody = null;
            attachedBombs.Clear();
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, layers);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, layers);

        if (slopeHitFront) {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        } else if (slopeHitBack) {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        } else {
            isOnSlope = false;
            slopeSideAngle = 0.0f;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, layers);

        if(hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle) {
            canWalkOnSlope = false;
        } else {
            canWalkOnSlope = true;
        }

        if (playerInput.x == 0 && !IsPushed()) {
            rb.sharedMaterial = fullFriction;
        } else {
            rb.sharedMaterial = noFriction;
        }
    }

    bool IsGrounded()
    {

        //Tjek om spilleren er på jorden
        return Physics2D.OverlapBox(transform.position + new Vector3(0.01f, -0.365f) * transform.localScale.y, new Vector2(0.2f, 0.125f), 0, layers);
    }

    private bool IsPlayerWallSliding()
    {
        return rb.velocity.y <= 0f && ((facingRight && player.GetKey(KeyMaps.Right)) || (!facingRight && player.GetKey(KeyMaps.Left))) && IsPlayerTouchingWall();
    }

    private bool IsPlayerTouchingWall()
    {
        Vector2 center = transform.position + new Vector3(facingRight ? 0.2f : -0.2f, -0.1f) * transform.localScale.x;
        return Physics2D.OverlapBox(center, new Vector2(0.125f, 0.4f), 0, layers);
    }

    private bool IsPlayerJumpedOn() {
       return Physics2D.OverlapBox(transform.position + new Vector3(0.01f, 0.27f) * transform.localScale.y, new Vector2(0.2f, 0.08f), 0, headLayer);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player" && IsPlayerJumpedOn() && CanBeJumpedOn) {
            IsJumpedOn = true;
        }
}

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Tegner box som ser om spilleren er på jorden
        if(IsGrounded())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(0.01f, -0.365f) * transform.localScale.y, new Vector2(0.2f, 0.125f));
        Gizmos.DrawWireCube(transform.position + new Vector3(0.01f, 0.27f) * transform.localScale.y, new Vector2(0.2f, 0.08f));

        if (IsPlayerTouchingWall())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.white;
        Vector2 center = transform.position + new Vector3(facingRight ? 0.2f : -0.2f, -0.1f) * transform.localScale.x;
        Gizmos.DrawWireCube(center, new Vector2(0.125f, 0.4f));
    }
#endif
}
