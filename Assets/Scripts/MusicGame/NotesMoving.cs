using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesMoving : MonoBehaviour
{
    private Vector2 startLocation;
    private Vector2 endLocation;
    
    private bool keepExtending = false;
    private bool keepShrinking = false;
    private musicNoteType type =  musicNoteType.Long;
    private musicNotesPosition pos = musicNotesPosition.A;
    
    public static float extendRate { get; private set; }
    public static float speed = 5;
    private float distance;
    public bool isOnSpot;
    private BoxCollider2D boxCollider;
    private bool isMissed;
    private float pressStartTime;
    private float unitToScaleRatio;

    

    private void Start()
    {
        isOnSpot = false;
        isMissed = true;
        extendRate = 2f;
        startLocation = transform.position;
        endLocation = startLocation + new Vector2(24, 0);
        distance = Vector2.Distance(startLocation, endLocation);
        StartCoroutine(Offset());
        boxCollider = GetComponent<BoxCollider2D>();
        float originalWidthWorld = GetComponent<SpriteRenderer>().bounds.size.x / transform.localScale.x;
        unitToScaleRatio = 1f / originalWidthWorld;

    }

    private void Update()
    {
        if (keepExtending)
        {
            Vector3 scale = transform.localScale;
            scale.x += 1f * Time.deltaTime * extendRate;
            transform.localScale = scale;

            AlignEnds();
        }

        if (keepShrinking)
        {
            Vector3 scale = transform.localScale;
            scale.x -= 1f * Time.deltaTime * extendRate;
            scale.x = Mathf.Max(scale.x, 0.1f); // prevent negative or zero scale
            transform.localScale = scale;

            AlignEnds();
        }
    }
    
    private void AlignEnds()
    {
        // Middle bar's size after scaling
        float barWorldWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        // Left stays at bar's start
        Transform left = transform.parent.Find("Left");
        if (left != null)
            left.localPosition = Vector3.zero;

        // Right moves to bar's end
        Transform right = transform.parent.Find("Right");
        if (right != null)
            right.localPosition = new Vector3(barWorldWidth, 0, 0);
    }


    public void StartExtending()
    {
        keepExtending = true;
    }
    
    public void StopExtending()
    {
        keepExtending = false;
    }

    private void StartMoving()
    {
        StartCoroutine(moveOut());
    }

    #region GettersNSetters

    public void SetType(musicNoteType newType)
    {
        type = newType;
    }
    
    public musicNoteType GetType()
    {
        return type;
    }

    public void setMissed(bool missed)
    {
        isMissed = missed;
    }

    public bool getMissed()
    {
        return isMissed;
    }
    
    public void SetPos(musicNotesPosition newPos)
    {
        pos = newPos;
    }
    
    public musicNotesPosition GetPos()
    {
        return pos ;
    }

    #endregion


    
    private IEnumerator moveOut()
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition - new Vector2(distance, 0);

        float elapsedTime = 0;
        float duration = distance / speed;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.parent.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.parent.position = endPosition;

        if (gameObject != null)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    
    private IEnumerator Offset()
    {
        yield return new WaitForSeconds(0);
        StartMoving();

    }
    


    public void SetPressStartTime(float time)
    {
        pressStartTime = time;
    }

    public float GetPressStartTime()
    {
        return pressStartTime;
    }
}
