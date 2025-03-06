using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class joinedPlayersText : MonoBehaviour
{

    private int playersJoined = 0;
    [SerializeField] Button playButton;
    [SerializeField] Button vsButton;
    public GameObject[] playerControls;

    private TMPro.TMP_Text playText;


    public bool isText;

    public void Awake()
    {
        if (isText)
        {
            StartCoroutine(BlinkingEffect());
            playText = playButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
        }
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerJoined();
        }
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerDisconnected();
        }
        if(playersJoined == 0)
        {
            playText.text = "Need a player";
            playButton.interactable = false;
            vsButton.interactable = false;
        }
        else if (playersJoined == 1)
        {
            playText.text = "Singleplayer";
            playButton.interactable = true;
            vsButton.interactable = false;
        }
        else
        {
            playText.text = "Co-op";
            vsButton.interactable = true;
            playButton.interactable = true;
        }


    }

    public void PlayerJoined() {
        if (playersJoined < 4) {
            
            joinedPlayers.playablePlayers++;
            playersJoined++;
            //if(isText) {
                transform.GetChild(playersJoined - 1).gameObject.SetActive(false);
            /*} else {
                  transform.GetChild(playersJoined - 1).gameObject.SetActive(true);
            }*/
            playerControls[playersJoined - 1].SetActive(true);

            //transform.GetChild(playersJoined - 1).GetComponent<TMPro.TMP_Text>().text = "Player: " + playersJoined;

        }
    }
    public void PlayerDisconnected()
    {
        if (playersJoined > 0)
        {
            if (playersJoined < 4)
            {
                transform.GetChild(playersJoined).gameObject.SetActive(false);
            }
            joinedPlayers.playablePlayers--;
            playersJoined--;
            transform.GetChild(playersJoined).gameObject.SetActive(false);
            playerControls[playersJoined].SetActive(false);

        }
    }



   
    public float timeActive = 1.6f;
    public float timeDeactived = 0.6f;

    IEnumerator BlinkingEffect()
    {
        while (true)
        {
            TextToBlink();
            yield return new WaitForSeconds(timeActive);
            TextToBlink();
            yield return new WaitForSeconds(timeDeactived);
        }
    }


    private void TextToBlink()
    {
        if(playersJoined == 4)
        { return; }
        int childToBlink = playersJoined;
        transform.GetChild(childToBlink).GetComponent<TMPro.TMP_Text>().text = "Press enter to join";
        transform.GetChild(childToBlink).gameObject.SetActive(!transform.GetChild(childToBlink).gameObject.activeInHierarchy);
        //transform.GetChild(childToBlink).GetComponent<TMPro.TMP_Text>().enabled = !transform.GetChild(childToBlink).GetComponent<TMPro.TMP_Text>().enabled;
    }
}
