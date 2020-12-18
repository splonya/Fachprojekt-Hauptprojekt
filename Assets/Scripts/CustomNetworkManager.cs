using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public Transform firstPosition;
    public Transform secondPosition;

    public float projectileVelocity = 1;

    GameObject ball;

    public Transform[] spawners;
    
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
            //ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Projectile"));
            //NetworkServer.Spawn(ball);

            StartCoroutine(onCoroutine());
        }
    }

    IEnumerator onCoroutine()
    {
        while (true)
        {
            var rnd = new System.Random();
            var spawnId = rnd.Next(0, spawners.Length);
            var directionSpawnId = (spawnId + spawners.Length / 2) % spawners.Length;

            Debug.LogError(spawners[spawnId].position);

            var projectile = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Projectile"), new Vector3(spawners[spawnId].position.x, spawners[spawnId].position.y, 0), Quaternion.identity);

            projectile.GetComponent<Rigidbody2D>().velocity = -Vector3.MoveTowards(spawners[spawnId].position, spawners[directionSpawnId].position, projectileVelocity * Time.deltaTime);

            Debug.Log($"start: {spawnId}, target: {directionSpawnId}");

            NetworkServer.Spawn(projectile);

            yield return new WaitForSeconds(5f);
        }
    }
}
