using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public Rigidbody2D rigidbody2d;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // only simulate ball physics on server
        rigidbody2d.simulated = true;

        // Serve the ball from left player
        //rigidbody2d.velocity = Vector2.right * 2;
    }

    // only call this on server
    [ServerCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.LogError("hereeeeeeeeeeeeeeeeee!");
        NetworkServer.Destroy(col.gameObject);
    }
}
