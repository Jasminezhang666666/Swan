using System.Collections;
using System.Collections.Generic;
using Fungus;
using Unity.VisualScripting;
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
    private bool pressedOnTimeA;
    private bool pressedOnTimeB;
    [SerializeField] private KeyStatus currentKeyStatus;
    private GameObject upKey;
    private GameObject downKey;

    private Dictionary<KeyStatus, int> playerScores = new Dictionary<KeyStatus, int>();
    private bool noteInCollisionA = false;
    private bool noteInCollisionB = false;
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
        if (upKey != null)HandleNoteInput(noteInCollisionA, upKey, KeyCode.W, animUp,  animatorUp);
        if(downKey != null)HandleNoteInput(noteInCollisionB, downKey, KeyCode.S, animDown,  animatorDown);
    }
    
    private void HandleNoteInput(bool noteInCollision, GameObject key, KeyCode keyCode, GameObject anim, Animator animator)
    {
        musicNoteType _type = key.GetComponent<NotesMoving>().GetType();
        if (noteInCollision)
        {
            if (Input.GetKeyDown(keyCode))
            {
                anim.SetActive(true);
                animator.speed = 1f;
                //mode = (keyCode == KeyCode.W) ? Keys.UP : Keys.DOWN;
                if (key.GetComponent<NotesMoving>().GetType() == musicNoteType.Short)
                {
                    Destroy(key.transform.parent.gameObject);
                    currentKeyStatus = KeyStatus.OK;
                    playerScores[currentKeyStatus]++;
                    //PrintScores();
                }
                else if (key.GetComponent<NotesMoving>().GetType() == musicNoteType.Long)
                {
                    key.transform.parent.transform.Find("Left").gameObject.GetComponent<Renderer>().enabled = false;
                    NoteMask mask = key.transform.parent.GetComponentInChildren<NoteMask>();
                    mask.StartExtending();
                    mask.marked = true;
                }
            }
            else if(Input.GetKeyUp(keyCode) && key.GetComponent<NotesMoving>().GetType() == musicNoteType.Long)
            {
                key.GetComponent<NotesMoving>()
                    .isOnSpot = false;
            }
            else if(!Input.anyKey)
            {
                NoteMask mask = key.transform.parent.GetComponentInChildren<NoteMask>();
                animator.gameObject.SetActive(false);
                animatorUp.speed = 0f;
                animatorDown.speed = 0f;
                mode = Keys.NULL;
                if (key.GetComponent<NotesMoving>().GetType() == musicNoteType.Long && mask.marked)
                {
                    mask.StopExtending();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LongNote"))
        {
            collision.gameObject.transform.parent.transform.Find("Note").GetComponent<NotesMoving>()
                .isOnSpot = true;
        }
        
        if (collision.gameObject.CompareTag("Note"))
        {
            bool isOnSpot = collision.gameObject.GetComponent<NotesMoving>().isOnSpot;
            if (collision.gameObject.GetComponent<NotesMoving>().GetType() == musicNoteType.Long && isOnSpot || collision.gameObject.GetComponent<NotesMoving>().GetType() == musicNoteType.Short)
            {
                musicNotesPosition _pos = collision.gameObject.GetComponent<NotesMoving>().GetPos();
                if (_pos == musicNotesPosition.A)
                {
                    noteInCollisionA = true;
                    upKey = collision.gameObject;
                }else if(_pos == musicNotesPosition.B)
                {
                    noteInCollisionB = true;
                    downKey = collision.gameObject;
                }
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            if (collision.gameObject.GetComponent<NotesMoving>().GetType() == musicNoteType.Long)
            {

                collision.gameObject.transform.parent.transform.Find("Note").GetComponent<NotesMoving>()
                    .isOnSpot = false;
            }
            if (!pressedOnTimeA || !pressedOnTimeB)
            {
                currentKeyStatus = KeyStatus.MISS;
                playerScores[KeyStatus.MISS]++;
                //PrintScores();
            }
            musicNotesPosition _exitPos = collision.gameObject.GetComponent<NotesMoving>().GetPos();
            if (_exitPos == musicNotesPosition.A)
            {
                pressedOnTimeA = false;
                noteInCollisionA = false;
            }else if(_exitPos == musicNotesPosition.B)
            {
                pressedOnTimeB = false;
                noteInCollisionB = false;
            }
            
        }else if (collision.gameObject.CompareTag("LongNoteEnd"))
        {
            collision.gameObject.transform.parent.GetComponentInChildren<NoteMask>().StopExtending();
        }
    }

    private void UpdateScore()
    {
        if (!pressedOnTimeA)
        {
            pressedOnTimeA = true;
            originalScale = upKey.transform.localScale;
            originalPosition = upKey.transform.position;

            //StartCoroutine(FadeOut(upKey.transform.parent.gameObject, originalScale, originalPosition));

        }
    }

    private void PrintScores()
    {
        foreach (KeyValuePair<KeyStatus, int> entry in playerScores)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

    public void PressLongNote(GameObject note)
    {
        GameObject noteMask = note.transform.Find("_Note").gameObject;
        noteMask.GetComponent<NoteMask>();
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
                oPosition.x + newScaleX - (oScale.x - newScaleX) / 2, 
                oPosition.y, 
                oPosition.z
            );
            yield return null;
        }
        note.SetActive(false);
    }

}
