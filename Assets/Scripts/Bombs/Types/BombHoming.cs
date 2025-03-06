using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHoming : MonoBehaviour {
    [Header("Myself")]
    public Bomb myself = null;
    private Rigidbody2D rb = null;
    private bool foundPlayer = false;
    private float originalGravity = 0;
    private float aliveTimer = 0f;
    private Vector2 targetPos;
    
    
    //Start is called before the first frame update
    void Start() {
        rb = myself.rb;
        originalGravity = rb.gravityScale;
        targetPos = transform.position;
    }

    //Homing stuff
    private void FixedUpdate() {
            
        //Tick this
        aliveTimer += Time.deltaTime;

        //Try to drop on top of player
        Vector2 force = Vector2.zero;
        float moveForce = 17f;
        float gravityForce = rb.velocity.y;
        rb.gravityScale = 0f;

        //Dive
        if(foundPlayer) {
            rb.gravityScale = originalGravity * 1.3f;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, 10f));
            moveForce = 30f;
        }

        //Find closest player (IF any exist, otherwise just use last seen player position)
        Player player = null;
        float lowestDistance = float.PositiveInfinity;
        foreach(Player i in Player.players) {
            float distanceToThis = Vector3.Distance(transform.position, i.transform.position);
            if(distanceToThis < lowestDistance) {
                lowestDistance = distanceToThis;
                player = i;
                targetPos = i.transform.position;
            }
        }

        //Move to the left
        if(transform.position.x > targetPos.x + 0.5f) {
            force.x = -moveForce;
        }

        //Move to the right
        else if(transform.position.x < targetPos.x - 0.5f) {
            force.x = moveForce;
        }

        //Initiate diving
        else if(aliveTimer >= 1f) foundPlayer = true;

        
        //Add force to rigidbody
        rb.AddForce(force, ForceMode2D.Force);
        if(!foundPlayer)
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -7, 7), Mathf.Lerp(gravityForce, -originalGravity, 0.1f));

        //Rotation
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
