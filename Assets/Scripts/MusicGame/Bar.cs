using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public enum Keys
    {
        LEFT,
        RIGHT,
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

    private Dictionary<KeyStatus, int> playerScores = new Dictionary<KeyStatus, int>();
    private bool noteInCollision = false;

    // Start is called before the first frame update
    void Start()
    {
        playerScores.Add(KeyStatus.OK, 0);
        playerScores.Add(KeyStatus.PERFECT, 0);
        playerScores.Add(KeyStatus.MISS, 0);
        mode = Keys.NULL;
    }

    // Update is called once per frame
    void Update()
    {
        mode = Keys.NULL;
        HandleInput();
    }
    private void HandleInput()
    {
        if (noteInCollision)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                mode = Keys.DOWN;
                UpdateScore();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                mode = Keys.UP;
                UpdateScore();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                mode = Keys.LEFT;
                UpdateScore();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                mode = Keys.RIGHT;
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
