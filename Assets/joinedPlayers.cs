using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joinedPlayers : MonoBehaviour
{ 
    public static int playablePlayers;
    public static bool versusMode = true;
    public static int playerWon = 0;
    public static int reachedEnd = -1;
    public static bool isFullscreen = true;
    public static int[] deaths = new int[] { 0, 0, 0, 0 };
    public static Resolution resolution;
    /// <summary>
    /// Can use this score to check against if the player has beat any high scores if that gets implemented. 
    /// </summary>
    public static int highestScoreGained;

    public static joinedPlayers Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            for (int i = 0; i < deaths.Length; i++)
            {
                deaths[i] = 0;
            }
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        isFullscreen = Screen.fullScreen;
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen, 0);
    }
}
