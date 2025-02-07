using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerAnimManager : MonoBehaviour
{
    public float leftLimit = -5f; // Left boundary
    public float rightLimit = 5f; // Right boundary
    public float moveSpeed = 2f; // Movement speed

    private bool movingRight = true; // Direction flag

    void Update()
    {
        // Move in the current direction
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            
            if (transform.position.x >= rightLimit)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            
            if (transform.position.x <= leftLimit)
            {
                movingRight = true;
            }
        }
    }
}
