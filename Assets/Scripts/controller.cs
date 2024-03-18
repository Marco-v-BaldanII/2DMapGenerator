using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {

        // Get input from arrow keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // Move the camera
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}

