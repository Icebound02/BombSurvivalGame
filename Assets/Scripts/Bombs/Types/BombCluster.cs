using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCluster : MonoBehaviour {
    
    [Header("Myself")]
    public Bomb myself = null;

    [Header("Child bombs")]
    public Bomb childBomb = null;
    public int bombCount = 2;
    public float childSpawnForce = 7.5f;
    
    //Runs whenever the object is destroyed (it explodes)
    private void OnDestroy() {

        //If object was destroyed some other way, don't trigger
        if(myself.hasExploded) {

            //Repeat for each child bomb needed
            float randSeed = Random.Range(0, Mathf.PI * 2f);
            for(int i = 0; i < bombCount; i++) {
                
                //Get position to spawn at
                float direction = randSeed + (Mathf.PI * 2f) / (float)bombCount * i;
                Vector2 directionVector = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)) * 0.1f;

                //Spawn child bomb and add force to it
                Bomb child = Instantiate(childBomb, transform.position + (Vector3)directionVector, Quaternion.identity);
                child.rb.AddForce(directionVector * childSpawnForce);

                //Set random phase delay in child
                child.bombTimer -= Random.Range(0f, 0.6f);
            }
        }
    }
}
