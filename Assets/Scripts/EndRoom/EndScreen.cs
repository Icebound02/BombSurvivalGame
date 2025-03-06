using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndScreen : MonoBehaviour
{
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.players.Remove(collision.GetComponent<Player>());

        string endReacher = collision.name.Replace("Player", "");

        joinedPlayers.reachedEnd = int.Parse(endReacher)-1;

     
        SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
    }

   
    
}
