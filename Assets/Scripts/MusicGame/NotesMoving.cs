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
    

    private void Start()
    {
        extendRate = 2;
        startLocation = transform.position;
        endLocation = startLocation + new Vector2(24, 0);
        distance = Vector2.Distance(startLocation, endLocation);
        StartCoroutine(Offset());
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(type == musicNoteType.Long) boxCollider.offset = new Vector3(0.5f * transform.localScale.x, 0, 0);
        if (keepExtending)
        {
            Vector3 scale = transform.localScale;
            scale.x += extendRate * Time.deltaTime; 
            transform.localScale = scale;
            transform.parent.Find("Right").transform.localPosition += new Vector3(extendRate * Time.deltaTime * 1.1f, 0, 0);
        }
        
        if (keepShrinking)
        {
            Vector3 scale = transform.localScale;
            scale.x -= extendRate * Time.deltaTime; 
            transform.localScale = scale;
            transform.parent.Find("Left").transform.localPosition -= new Vector3(extendRate * Time.deltaTime * 1.1f, 0, 0);
        }
        
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

    public void SetType(musicNoteType newType)
    {
        type = newType;
    }
    
    public musicNoteType GetType()
    {
        return type;
    }
    
    public void SetPos(musicNotesPosition newPos)
    {
        pos = newPos;
    }
    
    public musicNotesPosition GetPos()
    {
        return pos ;
    }

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
}
