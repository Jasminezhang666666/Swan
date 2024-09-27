using System.Collections;
using System.Collections.Generic;
using Fungus;
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
    [SerializeField] private GameObject animUp;
    [SerializeField] private GameObject animDown;
    private Animator animatorUp;
    private Animator animatorDown;

    private float speed;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    
    
    
    void Start()
    {
        playerScores.Add(KeyStatus.OK, 0);
        playerScores.Add(KeyStatus.PERFECT, 0);
        playerScores.Add(KeyStatus.MISS, 0);
        mode = Keys.NULL;
        animatorUp = animUp.GetComponent<Animator>();
        animatorUp.speed = 0f; 
        animatorDown = animDown.GetComponent<Animator>();
        animatorDown.speed = 0f;
        speed = NotesMoving.speed;
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
                animDown.SetActive(true); 
                animatorDown.speed = 1f; 
                mode = Keys.DOWN;
                UpdateScore();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                animUp.SetActive(true); 
                animatorUp.speed = 1f; 
                mode = Keys.UP;
                UpdateScore();
            }
            else
            {
                animUp.SetActive(false); 
                animDown.SetActive(false); 
                animatorUp.speed = 0f; 
                animatorDown.speed = 0f; 
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
            originalScale = upKey.transform.localScale;
            originalPosition = upKey.transform.position;
            Destroy(upKey.transform.parent.gameObject);
            //StartCoroutine(FadeOut(upKey.transform.parent.gameObject, originalScale, originalPosition));
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
    
    IEnumerator FadeOut(GameObject note, Vector3 originalScale, Vector3 originalPosition)
    {
        GameObject middleNote = note.transform.Find("Note").gameObject;
        Vector3 oScale = middleNote.transform.localScale;
        Vector3 oPosition = middleNote.transform.position;
        Destroy(note.transform.Find("Left").gameObject);
        while (middleNote != null && middleNote.transform.localScale.x > 0)
        {
            float newScaleX = middleNote.transform.localScale.x - (speed * Time.deltaTime);
            newScaleX = Mathf.Max(newScaleX, 0);
            middleNote.transform.localScale = new Vector3(newScaleX, oScale.y, oScale.z);
            middleNote.transform.position = new Vector3(
                oPosition.x - (oScale.x - newScaleX) / 2, 
                oPosition.y, 
                oPosition.z
            );
            yield return null;
        }
        note.SetActive(false);
    }

}
