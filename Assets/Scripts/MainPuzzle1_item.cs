using System.Runtime.InteropServices;
using UnityEngine;

public class MainPuzzle1_item : MonoBehaviour
{
    protected Vector3 offset;
    protected bool isDragging = false;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    protected Camera mainCamera;

    protected void Start()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("DraggableItem: No Collider2D component found. Please add a Collider2D component to make this object draggable.");
        }

        myRenderer = GetComponent<SpriteRenderer>();
        if (myRenderer == null)
        {
            Debug.LogError("DraggableItem: No SpriteRenderer component found. Z-order manipulation will not work.");
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

    protected void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            offset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // Optional: Bring the item to the front when selected
            if (myRenderer != null)
            {
                myRenderer.sortingOrder = 100; // Adjust the value to ensure it's in front of other objects
            }
        }
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;
        // Optional: Reset the item's sorting order if needed
        if (myRenderer != null)
        {
            myRenderer.sortingOrder = 0; // Adjust to the item's default layer
        }
    }
}
