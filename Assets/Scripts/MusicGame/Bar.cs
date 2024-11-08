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

    private bool isShaking = false;
    
    
    private void StopAnimationAndHide(Animator animator)
    {
        animator.speed = 0f;
        animator.gameObject.SetActive(false);
    }
    
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
    
    #region Input Handling
    
    private void HandleInput()
    {
        if (upKey != null)HandleNoteInput(noteInCollisionA, upKey, KeyCode.W,  animatorUp);
        if (downKey != null)HandleNoteInput(noteInCollisionB, downKey, KeyCode.S, animatorDown);
    }
    
    private void HandleNoteInput(bool noteInCollision, GameObject key, KeyCode keyCode, Animator animator)
    {
        var noteType = key.GetComponent<NotesMoving>().GetType();
        if (noteInCollision)
        {
            if (Input.GetKey(keyCode))
            {
                animator.gameObject.SetActive(true);
                //anim.SetActive(true);
                animator.speed = 1f;
                key.GetComponent<NotesMoving>().setMissed(false);
                if (noteType == musicNoteType.Short)
                {
                    Destroy(key.gameObject);
                    key.GetComponent<NotesMoving>().setMissed(false);
                    currentKeyStatus = KeyStatus.OK;
                    playerScores[currentKeyStatus]++;
                }
                else if (noteType == musicNoteType.Long)
                {
                    GameObject parent = key.transform.parent.gameObject;
                    parent.transform.Find("Left").gameObject.GetComponent<Renderer>().enabled = false;
                    NoteMask mask = parent.GetComponentInChildren<NoteMask>();
                    if (key.GetComponent<NotesMoving>()
                        .isOnSpot)
                    {
                        mask.StartExtending();
                        mask.marked = true;
                    }
                    
                }
            }
            //long note but stop in the middle
            else if(Input.GetKeyUp(keyCode) && noteType == musicNoteType.Long)
            {
                StopAnimationAndHide(animator);
                key.GetComponent<NotesMoving>()
                    .isOnSpot = false;
            }
            // else if(!Input.anyKey)
            // {
            //     NoteMask mask = key.transform.parent.GetComponentInChildren<NoteMask>();
            //     //anim.SetActive(false);
            //     StopAnimationAndHide(animator);
            //     //animator.gameObject.SetActive(false);
            //     mode = Keys.NULL;
            //     if (key.GetComponent<NotesMoving>().GetType() == musicNoteType.Long && mask.marked)
            //     {
            //         mask.StopExtending();
            //     }
            // }
        }
        else
        {
            NoteMask mask = key.transform.parent.GetComponentInChildren<NoteMask>();
            StopAnimationAndHide(animator);
            mode = Keys.NULL;
            // if (noteType == musicNoteType.Long && mask.marked)
            // {
            //     mask.StopExtending();
            // }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("LongNote"))
        {
            
            collision.gameObject.transform.parent.transform.Find("Note").GetComponent<NotesMoving>()
                .isOnSpot = true;
        }
        
        if (collision.gameObject.CompareTag("Note"))
        {
            var noteType = collision.GetComponent<NotesMoving>().GetType();
            bool isOnSpot = collision.gameObject.GetComponent<NotesMoving>().isOnSpot;
            if (noteType == musicNoteType.Long && isOnSpot ||
                noteType == musicNoteType.Short)
            {
                musicNotesPosition _pos = collision.gameObject.GetComponent<NotesMoving>().GetPos();
                if (_pos == musicNotesPosition.A)
                {
                    noteInCollisionA = true;
                    upKey = collision.gameObject;
                }
                else if (_pos == musicNotesPosition.B)
                {
                    noteInCollisionB = true;
                    downKey = collision.gameObject;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collisionNote = collision.gameObject;
        if (collision.gameObject.CompareTag("Note"))
        {
            if (collisionNote.GetComponent<NotesMoving>().getMissed())
            {
                currentKeyStatus = KeyStatus.MISS;
                playerScores[KeyStatus.MISS]++;
                //shake screen
                if (!isShaking)
                {
                    isShaking = true;
                    StartCoroutine(ShakeScreen(0.2f, 0.05f));
                    
                }
            }
            if (collisionNote.GetComponent<NotesMoving>().GetType() == musicNoteType.Long)
            {

                collisionNote.transform.parent.transform.Find("Note").GetComponent<NotesMoving>()
                    .isOnSpot = false;

            }
            musicNotesPosition _exitPos = collision.gameObject.GetComponent<NotesMoving>().GetPos();
            if (_exitPos == musicNotesPosition.A)
            {
                //pressedOnTimeA = false;
                noteInCollisionA = false;
            }else if(_exitPos == musicNotesPosition.B)
            {
                //pressedOnTimeB = false;
                noteInCollisionB = false;
            }
            
        }else if (collision.gameObject.CompareTag("LongNoteEnd"))
        {
            collision.gameObject.transform.parent.GetComponentInChildren<NoteMask>().StopExtending();
        }
        else if(collision.gameObject.CompareTag("LongNote"))
        {
            GameObject mainNote = collision.gameObject.transform.parent.transform.Find("Note").gameObject;
            mainNote.GetComponent<NotesMoving>()
                .isOnSpot = false;
            //shake screen
            if (mainNote.GetComponent<NotesMoving>().getMissed())
            {
                if (!isShaking)
                {
                    isShaking = true;
                    StartCoroutine(ShakeScreen(0.2f, 0.05f));
                    
                }
            }


        }
    }
    
    #endregion

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
    //object
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
    
    //camera
    public IEnumerator ShakeScreen(float duration, float intensity)
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * intensity;
            float offsetY = Random.Range(-1f, 1f) * intensity;

            Camera.main.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalPosition;
        isShaking = false;
    }
    
    
    

}
