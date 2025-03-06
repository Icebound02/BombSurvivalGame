using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager singleton;

    [SerializeField] private float startAltitude = default;

    public float lowestAltitude { private get; set; }
    public float lowestDepth => startAltitude - lowestAltitude;

    [NonSerialized] public float[] actionScores = new float[Player.MAX_PLAYERS];
    [NonSerialized] public float[] explorationScores = new float[Player.MAX_PLAYERS];
    [NonSerialized] public int[] carriedAliens = new int[Player.MAX_PLAYERS];
    [NonSerialized] public int[] deathScores = new int[Player.MAX_PLAYERS];

    private void Awake()
    {
        if(singleton != null && singleton != this)
        {
            Array.Clear(singleton.actionScores, 0, singleton.actionScores.Length);
            Array.Clear(singleton.explorationScores, 0, singleton.explorationScores.Length);
            Array.Clear(singleton.carriedAliens, 0, singleton.carriedAliens.Length);
            singleton.lowestAltitude = startAltitude;
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        lowestAltitude = startAltitude;
    }

    private void LateUpdate()
    {
        float localLowestAltitude = float.PositiveInfinity;
        for(int i = 0; i < Player.players.Count; ++i)
        {
            if(Player.players[i].transform.position.y < localLowestAltitude)
            {
                localLowestAltitude = Player.players[i].transform.position.y;
                if(localLowestAltitude < lowestAltitude)
                {
                    AddExplorationScore(Player.players[i], lowestAltitude - Player.players[i].transform.position.y);
                    lowestAltitude = Player.players[i].transform.position.y;
                }
            }
        }

        if(DepthMeter.singleton)
            DepthMeter.singleton.SetAltitude(localLowestAltitude);
    }

    public void AddActionScore(Player player, float amount)
    {
        actionScores[player.playerId] += amount;
        PlayerUI.singleton.actionScoreText[player.playerId].text = "Action\n" + Mathf.RoundToInt(actionScores[player.playerId]);
        ScoreTextManager.singleton.UpdateScoreText(amount, ScoreTextManager.ScoreTypes.Action, player);
    }

    public void AddExplorationScore(Player player, float amount)
    {
        explorationScores[player.playerId] += amount;
        PlayerUI.singleton.explorationScoreText[player.playerId].text = "Explored\n" + Mathf.RoundToInt(explorationScores[player.playerId]) + " m";
        ScoreTextManager.singleton.UpdateScoreText(amount, ScoreTextManager.ScoreTypes.Exploration, player);
    }

    public void AddRescueScore(Player player, int amount)
    {
        carriedAliens[player.playerId] += amount;
        PlayerUI.singleton.rescueScoreText[player.playerId].text = "Rescued\n" + Mathf.RoundToInt(carriedAliens[player.playerId]);
        ScoreTextManager.singleton.UpdateScoreText(amount, ScoreTextManager.ScoreTypes.Rescue, player);
    }

    public void AddDeathScore(Player player, int amount) {
        deathScores[player.playerId] += amount;
        PlayerUI.singleton.deathScoreText[player.playerId].text = "Deaths\n" + deathScores[player.playerId];
        ScoreTextManager.singleton.UpdateScoreText(amount, ScoreTextManager.ScoreTypes.Death, player);
    }
}
