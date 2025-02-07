using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerAnimManager : MonoBehaviour
{
    private float leftLimit = -5f; // Left boundary
    private float rightLimit = 5f; // Right boundary
    private float moveSpeed = 0f; // Movement speed

    private bool movingRight = true; 
    

    void Update()
    {
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
