using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public float speed;

    public Rigidbody2D rb;

    private Vector2 moveVelocity;

    [SyncVar(hook = nameof(SetColor))]
    private Color currentColor = Color.black;

    public override void OnStartServer()
    {
        base.OnStartServer();

        SetRandomColor();
    }

    private void SetRandomColor()
    {
        var colors = new List<Color>
        {
            Color.green,
            Color.red,
            Color.yellow,
            Color.blue
        };

        System.Random rnd = new System.Random();

        currentColor = colors[rnd.Next(0, colors.Count - 1)];
    }

    void SetColor(Color oldColor, Color newColor)
    {       
        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = newColor;
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogError(collision.gameObject.name);
    }
}
