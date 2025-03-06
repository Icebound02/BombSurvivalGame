using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveStoneScript : MonoBehaviour
{
    public Animator anim;
    [Tooltip("How long the surviving player has to stay inside the gravestone")]
    public float timeToStay = 2;
    private float timeStayed = 0;
    private bool spawned = false;
    public GameObject[] playerPrefabs;
    [System.NonSerialized] public string playerName;
    [System.NonSerialized] public KeyCode[] controls = new KeyCode[System.Enum.GetValues(typeof(KeyMaps)).Length];
    [SerializeField] private int playerLayer = default;
    private void OnTriggerStay2D(Collider2D collision)


    {
        if(collision.gameObject.layer == playerLayer)
        {
            timeStayed += Time.deltaTime;
            anim.Play("Revive");
        }
        if (timeStayed >= timeToStay && !spawned)
        {
            spawned = true;
            RessurectPlayer();
        }


    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer != playerLayer)
            return;

        timeStayed = 0;
        anim.Play("Idle");
    }

    private void RessurectPlayer()
    {
     playerName = playerName.Replace("Player", "");

        var player = Instantiate(playerPrefabs[int.Parse(playerName)-1], GetComponentInParent<Transform>().position,Quaternion.identity);
        player.name = "Player"+playerName;
        player.GetComponent<Player>().controls = controls;
        Destroy(transform.parent.gameObject);
       
    }

}
