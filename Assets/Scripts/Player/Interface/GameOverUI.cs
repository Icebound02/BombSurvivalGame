using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used for both GameOver and Win screen
/// </summary>
public class GameOverUI : MonoBehaviour
{
    private float explorationBuff = 1.5f;
    private List<int> playerScore = new List<int>();


    [Header("Points for players")]
    private float extraPointsForFinishing = 25;
    private float deathPoints = 15;
    private float carriedPoints = 10;

    public GameObject[] playerUI;
    /// <summary>
    /// Only appears in GameOver, not Win screen
    /// </summary>
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI[] actionScoreText;
    public TextMeshProUGUI[] explorationScoreText;
    public TextMeshProUGUI[] rescueScoreText;
    public TMP_Text[] totalScore;

    private void Start()
    {
        if (depthText)
            depthText.text = "Depth Reached: " + Mathf.RoundToInt(ScoreManager.singleton.lowestDepth) + " m";

        for (int i = 0; i < joinedPlayers.playablePlayers; ++i)
        {
            playerScore.Add(ScoreCalculation(i));
            playerUI[i].SetActive(true);
            actionScoreText[i].text = "Action\n" + Mathf.RoundToInt(ScoreManager.singleton.actionScores[i]);
            explorationScoreText[i].text = "Explored\n" + Mathf.RoundToInt(ScoreManager.singleton.explorationScores[i]) + " m";
            rescueScoreText[i].text = "Rescue\n" + Mathf.RoundToInt(ScoreManager.singleton.carriedAliens[i]);
            totalScore[i].text = "Total\n" + playerScore[i];

            transform.GetChild(0).GetChild(i).GetChild(4).gameObject.SetActive(true);
        }
        joinedPlayers.highestScoreGained = HighestScoreCalculation();

        //ændre farve
        UnityEngine.UI.ColorBlock colors = transform.GetChild(0).GetChild(joinedPlayers.playerWon).GetChild(7).GetComponent<UnityEngine.UI.Toggle>().colors;
        Color disabledColor = colors.disabledColor;
        disabledColor = Color.green;
        colors.disabledColor = disabledColor;



        if (joinedPlayers.reachedEnd != -1)
        {
            transform.GetChild(0).GetChild(joinedPlayers.playerWon).GetChild(4).GetComponent<UnityEngine.UI.Toggle>().colors = colors;
            transform.GetChild(0).GetChild(joinedPlayers.playerWon).GetChild(4).GetComponent<UnityEngine.UI.Toggle>().isOn = true;
            transform.GetChild(0).GetChild(joinedPlayers.playerWon).GetChild(4).GetChild(0).GetChild(0).gameObject.SetActive(true);

        }

        transform.GetChild(0).GetChild(joinedPlayers.playerWon).GetChild(6).gameObject.SetActive(true);
        joinedPlayers.reachedEnd = -1;
        for (int i = 0; i < joinedPlayers.deaths.Length; i++)
        {
            joinedPlayers.deaths[i] = 0;
        }
    }


    /// <summary>
    /// Calculates points for player i
    /// </summary>
    /// <param name="i">Player Int</param>
    /// <returns></returns>
    private int ScoreCalculation(int i)
    {
        if(joinedPlayers.reachedEnd == i && joinedPlayers.reachedEnd >-1)
        {
            extraPointsForFinishing = 25;
        }
        else
        {
            extraPointsForFinishing = 0;
        }
        float actionScore = ScoreManager.singleton.actionScores[i];
        float explorationScore = ScoreManager.singleton.explorationScores[i];
        float rescureScore = ScoreManager.singleton.carriedAliens[i] * carriedPoints;
        float deathScore = joinedPlayers.deaths[i] * deathPoints;

     int tempScore = Mathf.RoundToInt(actionScore + (explorationScore * explorationBuff) + extraPointsForFinishing - deathScore + rescureScore);
        if(tempScore < 0)
        {
            tempScore = 0;
        }
        return tempScore;
    }


    /// <summary>
    /// Tells JoinedPlayers who won (playerwon), and returns the highest score achieved. 
    /// </summary>
    /// <returns></returns>
    private int HighestScoreCalculation()
    {
        int highest = playerScore[0];

        for (int i = 0; i < playerScore.Count; i++)
        {
            if (playerScore[i] > highest)
            {
                highest = playerScore[i];
                joinedPlayers.playerWon = i;

            }
        }
        return highest;
    }

}
