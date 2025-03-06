using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public const int MAX_PLAYERS = 4;

    /*public static readonly Color[] playerColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };*/
    public Animator TorchAnim;
    public static float halfWidth;
    [SerializeField] int playerRespawnTime = 1;
    [SerializeField] GameObject graveStone;
    public static List<Player> players = new List<Player>();
    [System.NonSerialized] public int playerId;

    [System.NonSerialized] public KeyCode[] controls = new KeyCode[System.Enum.GetValues(typeof(KeyMaps)).Length];

    public PlayerMovement movement;
    public PlayerAdaptiveDifficulty adaptiveDifficulty;
    public PlayerPowerup powerup;
    public Collider2D collider;
    public AudioSource audioSource;

    [Tooltip("Assign in same order as ScoreTextManager.ScoreTypes")]
    public ScoreTextAnimator[] scoreAnimators;

    [Header("DEBUG ONLY")]
    [SerializeField] private bool godmode = default;

    [System.NonSerialized] public List<Carryable> carriedAliens = new List<Carryable>();

    private void Awake()
    {
        halfWidth = 0f; // collider.bounds.extents.x;

        playerId = players.Count;
        players.Add(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Controls.UpdateControls(this);
    }

    public bool GetKey(KeyMaps input)
    {
        return Input.GetKey(controls[(int)input]);
    }
    public bool GetKeyDown(KeyMaps input)
    {
        return Input.GetKeyDown(controls[(int)input]);
    }
    public float GetAxis()
    {
        float value = 0f;
        if (GetKey(KeyMaps.Left))
            value -= 1f;
        else if (GetKey(KeyMaps.Right))
            value += 1f;
        return value;
    }
    public void Die()
    {
        if (godmode)
            return;

        players.Remove(this);



        int playerInt = int.Parse(gameObject.name.Replace("Player", "")) - 1;
        joinedPlayers.deaths[playerInt]++;

        bool vs = joinedPlayers.versusMode;
        if (!vs)
        {

            var graveClone = Instantiate(graveStone, transform.position, transform.rotation);
            graveClone.transform.GetChild(0).GetComponent<GraveStoneScript>().playerName = gameObject.name;
            graveClone.transform.GetChild(0).GetComponent<GraveStoneScript>().controls = controls;
        }

        //Hook to versus mode variable


        //Coop & singleplayer mode
        if (players.Count == 0 && !vs)
        {
            ScenemanagerScript.singleton.StopCoroutine(ScenemanagerScript.singleton.DeathSlowmo());
            ScenemanagerScript.singleton.StartCoroutine(ScenemanagerScript.singleton.DeathSlowmo());
            ScenemanagerScript.singleton.Invoke("LoadSceneGameOver", 0.5f);
        
        }

        //Drop all carried aliens
        float randSeed = Random.Range(0, Mathf.PI * 2f);
        float totalCount = carriedAliens.Count;
        for(int i = 0; carriedAliens.Count > 0; i++) {

            Debug.Log($"starting {i}");
            Debug.Log($"aliens left {carriedAliens.Count}");

            //Drop alien
            Carryable alien = carriedAliens[0];
            alien.Drop();
            Debug.Log($"dropped {i}");

            //Get direction to shoot out in
            float direction = randSeed + (Mathf.PI * 2f) / totalCount * i;
            Vector2 directionVector = new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)) * 2;

            //Shoot out alien
            alien.rb.AddForce(directionVector);
            Debug.Log($"set direction {i}");
        }

        // Ressureets player in vs mode 
        if (vs)
        {
            RespawnManagerVS.singleton.ResPlayer(gameObject.transform,controls);
        }
        ScoreManager.singleton.AddDeathScore(this, 1);

        Destroy(gameObject);
    }
}