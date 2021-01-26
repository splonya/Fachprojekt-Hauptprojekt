using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private float projectileVelocity = 1;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

    private List<Transform> spawners = new List<Transform>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
            Debug.LogWarning(prefab.name);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        bool isLeader = RoomPlayers.Count == 0;

        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

        roomPlayerInstance.IsLeader = isLeader;

        if (!isLeader)
        {
            roomPlayerInstance.gameObject.SetActive(false);
        }

        NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (!IsReadyToStart()) { return; }

        ServerChangeScene("SampleScene");
    }

    public override void ServerChangeScene(string newSceneName)
    {
        for (int i = RoomPlayers.Count - 1; i >= 0; i--)
        {
            var conn = RoomPlayers[i].connectionToClient;
            var gameplayerInstance = Instantiate(gamePlayerPrefab);
            gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

            NetworkServer.Destroy(conn.identity.gameObject);

            NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        spawners = GameObject.FindGameObjectsWithTag("Respawn").Select(x => x.transform).ToList();
        StartCoroutine(onCoroutine());
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    IEnumerator onCoroutine()
    {
        while (true)
        {
            if (numPlayers == 0)
            {
                break;
            }

            var rnd = new System.Random();
            var spawnId = rnd.Next(0, spawners.Count);
            var directionSpawnId = (spawnId + spawners.Count / 2) % spawners.Count;

            Debug.LogError(spawners[spawnId].position);

            var projectile = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Projectile"), new Vector3(spawners[spawnId].position.x, spawners[spawnId].position.y, 0), Quaternion.identity);

            projectile.GetComponent<Rigidbody2D>().velocity = -Vector3.MoveTowards(spawners[spawnId].position, spawners[directionSpawnId].position, projectileVelocity * Time.deltaTime);

            var multiplier = (float)(rnd.NextDouble() + 0.1);

            projectile.GetComponent<Rigidbody2D>().velocity *= projectileVelocity * multiplier;

            Debug.Log($"start: {spawnId}, target: {directionSpawnId}");

            NetworkServer.Spawn(projectile);

            var delayTime = rnd.Next(1, 3);

            yield return new WaitForSeconds(delayTime);
        }
    }
}
