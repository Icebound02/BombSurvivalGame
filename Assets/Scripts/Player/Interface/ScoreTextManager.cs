using UnityEngine;
using System.Collections.Generic;

public class ScoreTextManager : MonoBehaviour
{
    public static ScoreTextManager singleton;

    public enum ScoreTypes
    {
        Action,
        Exploration,
        Rescue,
        Death
    }
    private static readonly string[] scoreTypeStrings = new string[]
    {
        "Action",
        "Explored",
        "Rescue",
        "Died"
    };

    //[SerializeField] private GameObject prefabScoreText = default;
    //private List<ScoreTextAnimator>[] playerTexts = new List<ScoreTextAnimator>[Player.MAX_PLAYERS];

    private void Awake()
    {
        singleton = this;

        // Init lists in array
        //for(int i = 0; i < playerTexts.Length; ++i)
        //    playerTexts[i] = new List<ScoreTextAnimator>();
    }

    /*private ScoreTextAnimator RetrieveText(string searchBy)
    {
        for(int i = 0; i < playerTexts.Length; ++i)
        {
            for(int j = 0; j < playerTexts[i].Count; ++j)
            {
                if(!playerTexts[i][j].gameObject.activeSelf)
                    continue;
                if(playerTexts[i][j].text.text.Contains(searchBy))
                    return playerTexts[i][j];
            }
        }
        return null;
    }*/

    public void UpdateScoreText(float amount, ScoreTypes scoreType, Player player)
    {
        if ((int)scoreType >= player.scoreAnimators.Length) {
            player.scoreAnimators[(int)scoreType].StartAnimating(amount, scoreTypeStrings[(int)scoreType]);
        }


        /*ScoreTextAnimator textAnimator = RetrieveText(scoreType); // Get an existing text
        if(!textAnimator) // Spawn new text
        {
            GameObject newObj = Instantiate(prefabScoreText, player.transform.position, Quaternion.identity, transform);
            textAnimator = newObj.GetComponent<ScoreTextAnimator>();
            playerTexts[player.playerId].Add(textAnimator);
        }
        textAnimator.StartAnimating(amount, scoreType);*/

    }
}
