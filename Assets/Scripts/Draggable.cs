using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    [SerializeField] public Sprite highlightSpr;
    [HideInInspector] public Sprite normalSpr;
    protected Vector3 offset;
    protected bool isDragging = false;
    protected Camera mainCamera;

    protected void Awake()
    {
        normalSpr = GetComponent<SpriteRenderer>().sprite;

        //Debug Message:
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError("No BoxCollider2D attached to the GameObject");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("DraggableItem: Main Camera not found. Please ensure your scene has a main camera.");
        }
    }

    protected void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
        }
    }

    protected void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().sprite = highlightSpr;
        if (Input.GetMouseButtonDown(0)) //Pick up when pressing the left mouse button
        {
            isDragging = true;
            offset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    protected void OnMouseUp()
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sprite = normalSpr;
    }

    protected void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = normalSpr;
    }
}
