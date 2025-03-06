using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManagerVS : MonoBehaviour
{
    public static RespawnManagerVS singleton;
    private List<IEnumerator> respawnCoroutine = new List<IEnumerator>();
    public GameObject[] players;
    public float respawnTime = 3;
    [System.NonSerialized] public KeyCode[] controls = new KeyCode[System.Enum.GetValues(typeof(KeyMaps)).Length];


    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
        }

        for (int i = 0; i < 4; i++)
        {
            AddPlayer(i);
        }
    }

  

  public void ResPlayer(Transform player, KeyCode[] controls = null)
    {
        Vector3 tempPlayerPos = transform.position;
        Vector3 tempPlayerRot = transform.eulerAngles;
        int playerInt = int.Parse(player.gameObject.name.Replace("Player", ""))-1;
        respawnCoroutine[playerInt] = RespawnPlayer(playerInt,controls);
        StartCoroutine(respawnCoroutine[playerInt]);
        StartCoroutine(RespawnTimerUI(playerInt));
    }

    private void AddPlayer(int playerInt)
    {
        if(respawnCoroutine.Count <= 3)
        {
            respawnCoroutine.Add(RespawnPlayer(playerInt,null));
        }
    }

    private IEnumerator RespawnPlayer(int playerInt, KeyCode[] controls )
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            GameObject newPlayer = Instantiate(players[playerInt],new Vector2(11,35),Quaternion.identity);
            newPlayer.GetComponent<Player>().controls = controls;
            newPlayer.gameObject.name = "Player" + (playerInt+1);
            newPlayer.GetComponent<Player>().playerId = playerInt;
            StopCoroutine(respawnCoroutine[playerInt]);
        }
    }
    private IEnumerator RespawnTimerUI(int playerInt) {
        float time = respawnTime+0.5f;
        PlayerUI.singleton.respawnTimer[playerInt].gameObject.SetActive(true);

        while (time >= 0) {
            time -= Time.deltaTime;
            PlayerUI.singleton.respawnTimer[playerInt].text = time.ToString("#");
            yield return null;
        }
        PlayerUI.singleton.respawnTimer[playerInt].gameObject.SetActive(false);
    }


}
