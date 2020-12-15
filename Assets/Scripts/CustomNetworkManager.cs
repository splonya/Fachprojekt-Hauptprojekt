using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Transform firstPosition;
    public Transform secondPosition;

    public Transform TESTpos;
    GameObject ball;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var positionToSpawn = numPlayers == 0 ? firstPosition : secondPosition;
        var player = Instantiate(playerPrefab, positionToSpawn.position, positionToSpawn.rotation);

        //if (numPlayers > 0)
        //{
        //    player.GetComponent<SpriteRenderer>().color = Color.green;
        //}

        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers == 1)
        {
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Square"));
            NetworkServer.Spawn(ball);
        }
    }
}
