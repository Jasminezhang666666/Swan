using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesMoving : MonoBehaviour
{
    private Vector2 startLocation;
    private Vector2 endLocation;
    
    private bool keepExtending = true;
    
    [SerializeField] private float extendRate;
    public static float speed = 3;
    [SerializeField] private float distance;

    private void Start()
    {
        startLocation = transform.position;
        endLocation = startLocation + new Vector2(24, 0);
        distance = Vector2.Distance(startLocation, endLocation);
        
        StartMoving();
    }

    private void Update()
    {
        if (keepExtending)
        {
            Vector3 scale = transform.localScale;
            scale.x += extendRate * Time.deltaTime; 
            transform.localScale = scale;
            transform.parent.Find("Left").transform.localPosition -= new Vector3(extendRate * Time.deltaTime * 1.1f, 0, 0);
        }
        //transform.position = Vector2.Lerp(startLocation, endLocation, time);
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

    private IEnumerator moveOut()
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition - new Vector2(distance, 0);

        float elapsedTime = 0;
        while (elapsedTime < distance / speed)
        {
            transform.parent.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime * speed) / distance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
