using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public enum Keys
    {
        UP,
        DOWN,
        NULL
    }

    public enum KeyStatus
    {
        OK,
        PERFECT,
        MISS
    }

    private Keys mode;
    [SerializeField] private bool pressedOnTime;
    [SerializeField] private KeyStatus currentKeyStatus;
    private GameObject upKey;
    private GameObject downKeu;

    private Dictionary<KeyStatus, int> playerScores = new Dictionary<KeyStatus, int>();
    private bool noteInCollision = false;
    
    void Start()
    {
        playerScores.Add(KeyStatus.OK, 0);
        playerScores.Add(KeyStatus.PERFECT, 0);
        playerScores.Add(KeyStatus.MISS, 0);
        mode = Keys.NULL;
    }
    
    void Update()
    {
        mode = Keys.NULL;
        HandleInput();
    }
    private void HandleInput()
    {
        if (noteInCollision)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                mode = Keys.DOWN;
                UpdateScore();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                mode = Keys.UP;
                UpdateScore();
            }
            else
            {
                mode = Keys.NULL;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            noteInCollision = true;
            upKey = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            if (!pressedOnTime)
            {
                currentKeyStatus = KeyStatus.MISS;
                playerScores[KeyStatus.MISS]++;
                PrintScores();
            }
            pressedOnTime = false;
            noteInCollision = false;
        }
    }

    private void UpdateScore()
    {
        if (!pressedOnTime)
        {
            pressedOnTime = true;
            Destroy(upKey.transform.parent.gameObject);
            //Destroy(upKey);
            currentKeyStatus = KeyStatus.OK; 
            playerScores[currentKeyStatus]++;
            PrintScores();
        }
    }

    private void PrintScores()
    {
        foreach (KeyValuePair<KeyStatus, int> entry in playerScores)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

}
