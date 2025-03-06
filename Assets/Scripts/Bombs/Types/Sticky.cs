using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V1king;

public class Sticky : MonoBehaviour
{
    [System.NonSerialized] public List<FixedJoint2D> joints = new List<FixedJoint2D>();
    [SerializeField] float timeBeforeSticking = 0.5f;
    public bool canStick = true;
    [SerializeField] private int playerLayer = default;
    [SerializeField] private int levelBoundaryLayer = default;

    [SerializeField] private AudioSource audioSource = default;
    [SerializeField] private AudioClip audioStick = default;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(canStick)
        {
            if(collision.gameObject.layer == levelBoundaryLayer) // Dont stick to level boundary walls
                return;

            FixedJoint2D newJoint = gameObject.AddComponent(typeof(FixedJoint2D)) as FixedJoint2D;
            newJoint.connectedBody = collision.rigidbody;
            joints.Add(newJoint);
            if(collision.gameObject.layer == playerLayer)
            {
                PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                playerMovement.attachedBombs.Add(this);
            }

            // Sound
            audioSource.PlayOneShot(audioStick);
            // Animation
            Vector2 dir = (collision.GetContact(0).point - (Vector2)transform.position).normalized;
            ObjectPooler.singleton.SpawnStickyAnim(transform, VectorConversions.GetAngleFromVector(dir));
        }
    }

    private void LateUpdate()
    {
        if(joints.Count == 0)
            return;
        for(int i = joints.Count - 1; i >= 0; --i)
        {
            if(!joints[i].attachedRigidbody)
            {
                Destroy(joints[i]);
                joints.RemoveAt(i);
            }
        }
    }

    public IEnumerator TimerForStickAgain()
    {
        canStick = false;
        yield return new WaitForSeconds(timeBeforeSticking);
        canStick = true;
        StopAllCoroutines();
    }
}
