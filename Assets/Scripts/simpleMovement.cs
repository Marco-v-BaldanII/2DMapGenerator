using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleMazeMovement : MonoBehaviour
{


    public float moveSpeed = 5f; // Movement speed of the player

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player GameObject
    }

    void Update()
    {
        // Input handling for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        // Apply movement to the Rigidbody2D component
        rb.velocity = movement * moveSpeed;
    }



}
