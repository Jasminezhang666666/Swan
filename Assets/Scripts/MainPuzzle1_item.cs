using System.Collections;
using UnityEngine;

public class MainPuzzle1_item : MonoBehaviour
{
    protected Vector3 offset;
    protected bool isDragging = false;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    protected Camera mainCamera;

    // Static variable to keep track of the maximum sorting order
    private static int maxSortingOrder = 0; // Start from 0

    protected void Start()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("MainPuzzle1_item: No Collider2D component found. Please add a Collider2D component to make this object draggable.");
        }

        myRenderer = GetComponent<SpriteRenderer>();
        if (myRenderer == null)
        {
            Debug.LogError("MainPuzzle1_item: No SpriteRenderer component found. Z-order manipulation will not work.");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MainPuzzle1_item: Main Camera not found. Please ensure your scene has a main camera.");
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

    protected virtual void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            offset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Bring the item to the front when selected
            if (myRenderer != null)
            {
                myRenderer.sortingOrder = 100; // Temporarily set to high value while dragging
            }
        }
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;

        // Increase the max sorting order and set this item's sorting order to be the new max
        if (maxSortingOrder < 1000) // Limit the sorting order to 1000
        {
            maxSortingOrder++;
            if (myRenderer != null)
            {
                myRenderer.sortingOrder = maxSortingOrder; // Set to the new max sorting order
            }
        }
        else
        {
            // Reset maxSortingOrder or handle overflow logic if needed
            Debug.LogWarning("Max sorting order reached. No more items can be brought to the front.");
        }
    }

    // Optional: Use this method to clamp position within wall boundaries
    protected Vector3 ClampPositionWithinWalls(Vector3 position)
    {
        // Implement your clamping logic based on wall colliders here
        return position; // Return the clamped position
    }
}
