using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public Rigidbody2D rigidbody2d;

    public override void OnStartServer()
    {
        base.OnStartServer();
        rigidbody2d.simulated = true;
    }

    // only call this on server
    [ServerCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        NetworkServer.Destroy(col.gameObject);
    }
}
