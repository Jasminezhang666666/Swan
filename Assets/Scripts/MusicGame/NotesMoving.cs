using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesMoving : MonoBehaviour
{
    private Vector2 startLocation;

    private Vector2 endLocation;

    [SerializeField]
    private float time;
    private float minTime = 1;
    private float maxTime = 3;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float distance;

    private void Start()
    {
        startLocation = transform.position;
        endLocation = startLocation + new Vector2(24, 0);
        distance = Vector2.Distance(startLocation, endLocation);
        //speed = Random.Range(3, 6);
        StartMoving();
        //print(distance);
    }

    private void Update()
    {
        time = Random.Range(minTime, maxTime);
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartMoving();
        }
        //transform.position = Vector2.Lerp(startLocation, endLocation, time);
    }

    public void Initialize(float width)
    {
        Vector3 newScale = GetComponent<Transform>().localScale;
        newScale.x = width * speed;
        GetComponent<Transform>().localScale = newScale;
        
    }

    private void StartMoving()
    {
        StartCoroutine(moveOut());
    }

    private IEnumerator moveOut()
    {
        //yield return new WaitForSeconds(2f);
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + new Vector2(distance, 0);

        float elapsedTime = 0;
        while (elapsedTime < distance / speed)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime * speed) / distance);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
