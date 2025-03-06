using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI singleton;

    public Image[] playerUI;
    public TextMeshProUGUI[] actionScoreText;
    public TextMeshProUGUI[] explorationScoreText;
    public TextMeshProUGUI[] rescueScoreText;
    public TextMeshProUGUI[] deathScoreText;
    public TextMeshProUGUI[] respawnTimer;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        for(int i = 0; i < spawnPlayersManager.singleton.playersToSpawn; ++i)
            playerUI[i].gameObject.SetActive(true);
    }
}
