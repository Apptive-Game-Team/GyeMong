using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemo : SingletonObject<PlayerDemo>
{
    private float speed = 20f;
    private Rigidbody2D playerRigidbody;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(inputX, inputY).normalized * speed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + movement);
    }
}
