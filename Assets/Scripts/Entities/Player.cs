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

    public Animator animator;

    Vector3 characterScale;
    float characterScaleX;

    [SyncVar(hook = nameof(SetColor))]
    private Color currentColor = Color.black;


    void Start()
    {
        characterScale = transform.localScale;
        characterScaleX = characterScale.x;

    }


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
        {
            if(Input.GetAxis("Horizontal") != 0)
            {
                Debug.Log(Input.GetAxis("Horizontal"));
                animator.SetBool("IsRunning", true);
            }
            else
            {
                Debug.Log("DRIIIIINNNNN");
                animator.SetBool("IsRunning", false);
            }
            
           
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), 0) * speed * Time.fixedDeltaTime;

            if (Input.GetAxis("Horizontal") < 0)
            {
                characterScale.x = -characterScaleX;
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                characterScale.x = characterScaleX;
            }
            transform.localScale = characterScale;
        }
          
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.LogError(collision.gameObject.name);
    }
}
