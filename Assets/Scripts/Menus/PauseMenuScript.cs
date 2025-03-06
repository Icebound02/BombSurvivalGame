using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject controlsMenuUI;
    int numOfPlayers = joinedPlayers.playablePlayers;

    public GameObject player1Controls;
    public GameObject player2Controls;
    public GameObject player3Controls;
    public GameObject player4Controls;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        numOfPlayers = 4;

        if (Input.GetKeyDown(KeyCode.Escape) && !optionsMenuUI.activeInHierarchy) {
            if (gameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }

        switch (numOfPlayers) {
            case 1:
                player2Controls.SetActive(false);
                player3Controls.SetActive(false);
                player4Controls.SetActive(false);
                break;

            case 2:
                player3Controls.SetActive(false);
                player4Controls.SetActive(false);
                break;

            case 3:
                player4Controls.SetActive(false);
                break;
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMenu() {
        Player.players.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadOptions() {
        if (pauseMenuUI.activeInHierarchy) {
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }
        if (controlsMenuUI.activeInHierarchy) {
            controlsMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }
    }

    public void LoadPauseMenu() {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true); 
    }

    public void LoadControls() {
        optionsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);
    }
    
    public void QuitGame() {
        Application.Quit();
    }
}
