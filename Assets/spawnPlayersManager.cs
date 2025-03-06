using UnityEngine;

public class spawnPlayersManager : MonoBehaviour
{
    public static spawnPlayersManager singleton;

    /// <summary>
    /// How many players are getting spawned
    /// </summary>
    public int playersToSpawn { get; private set; } = 0;
    /// <summary>
    /// How many players have already been spawned
    /// </summary>
    private int playersSpawned = 0;

    [Tooltip("Don't touch, unless empty")]
    [SerializeField] Transform[] spawners;
    [Header("Player prefabs")]
    [Tooltip("If different player sprites for each player")]
    [SerializeField] GameObject[] playerPrefabs;

    private void Awake()
    {
        singleton = this;

        playersToSpawn = joinedPlayers.playablePlayers;
    
        for (int i = 0; i < playersToSpawn; i++)
        {
            SpawnPlayer(i);
        }
    }

    /// <summary>
    /// Spawns in players
    /// </summary>
    private void SpawnPlayer(int i)
    {
        GameObject player = Instantiate(playerPrefabs[i], spawners[i].position, transform.rotation);
        player.name = "Player" + (i+1);
        playersSpawned++;
    }
}
