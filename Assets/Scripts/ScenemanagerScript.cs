using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenemanagerScript : MonoBehaviour
{
    public static bool loading = false;
    public static ScenemanagerScript singleton;
    [Header("Only assign on main menu scene, otherwise leave empty")]
    [SerializeField] GameObject options;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject multiplayerMenu;
    private bool showingMainMenu = true;

    private void Awake()
    {
        singleton = this;
    }


    public void LoadMultiplayerMenu()
    {
        joinedPlayers.playablePlayers = 0;
        multiplayerMenu.SetActive(true);
        mainMenu.SetActive(false);
        showingMainMenu = false;
        
    }
    public void LoadSceneRestartGame()
    {
        if (joinedPlayers.playablePlayers > 0 && !loading)
        {
            StartCoroutine(LoadYourAsyncScene(1));
            Time.timeScale = 1f;
            PPController.singleton.SetVignette(0f);
        }
    }
    public void LoadSceneStartGame(bool versus)
    {
        if (joinedPlayers.playablePlayers > 0 && !loading)
        {
            joinedPlayers.versusMode = versus;
            //SceneManager.LoadScene(1);
            StartCoroutine(LoadYourAsyncScene(1));
            Time.timeScale = 1f;
            PPController.singleton.SetVignette(0f);
        }
    }

    private void Update()
    {

    }
    IEnumerator LoadYourAsyncScene(int sceneID) {

        //Start loading the scene
        loading = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);

        //Activate loading screen hopefully?
        LoadingScreen.singleton.Show();

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            loading = false;
            yield return null;
        }
    }

    public void ToggleOptionsMenu()
    {
        showingMainMenu = !showingMainMenu;
        if(showingMainMenu)
        {
            options.SetActive(false);
            mainMenu.SetActive(true);
            multiplayerMenu.SetActive(false);
        }
        else
        { 
            options.SetActive(true);
            mainMenu.SetActive(false);
        }

    }

    public void LoadSceneMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        PPController.singleton.SetVignette(0f);
    }
    public void LoadSceneGameOver()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1f;
        PPController.singleton.SetVignette(0f);
    }
    public void LoadSceneWinScene()
    {
        SceneManager.LoadScene("WinScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator DeathSlowmo()
    {
        const float duration = 1f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0.25f, 1f, time / duration);
            yield return null;
        }
    }
}
