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

    private float lifetimeLeft = 20;
    private void Update()
    {
        lifetimeLeft -= Time.deltaTime;

        if (lifetimeLeft <= 0)
        {
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }

    // only call this on server
    [ServerCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("projectile"))
        {
            NetworkServer.Destroy(col.gameObject);
        }
    }
}
