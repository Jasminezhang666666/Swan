using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPuzzle1_item : MonoBehaviour
{
    protected Vector3 offset;
    protected bool isDragging = false;
    protected Collider2D myCollider;
    protected SpriteRenderer myRenderer;
    protected Camera mainCamera;
    private Rigidbody2D rb; // Rigidbody2D reference
    private ItemSortingManager sortingManager;
    private Vector3 targetPosition;

    protected void Start()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("No Collider2D component found. Please add one.");
        }

        myRenderer = GetComponent<SpriteRenderer>();
        if (myRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found.");
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found.");
        }

        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found. Please add one.");
        }
        else
        {
            rb.isKinematic = true; // Ensure it is kinematic for dragging
        }

        sortingManager = FindObjectOfType<ItemSortingManager>();
        sortingManager.RegisterItem(this);
    }
    

    
    protected void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPosition = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
            Vector3 newPosition = transform.position;

            // Check if the full movement is valid
            if (CanMoveTo(targetPosition))
            {
                newPosition = targetPosition;
            }
            else
            {
                // Try horizontal movement only
                Vector3 horizontalMove = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
                if (CanMoveTo(horizontalMove))
                {
                    newPosition.x = horizontalMove.x;
                }
            
                // Try vertical movement only
                Vector3 verticalMove = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                if (CanMoveTo(verticalMove))
                {
                    newPosition.y = verticalMove.y;
                }
            }
        
            rb.MovePosition(newPosition);
        }
    }
    

    

    protected virtual void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            offset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            sortingManager.BringToFront(this); // Bring this item to front immediately
        }
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;
        // Notify manager that item has been dropped, even if it's dropped because of a wall
        sortingManager.ItemDropped(this);
    }

    public void SetSortingOrder(int order)
    {
        if (myRenderer != null)
        {
            myRenderer.sortingOrder = order; // Set the sorting order for this item
        }
    }

    private bool CanMoveTo(Vector3 targetPosition)
    {
        // Check for walls using a small overlap circle
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPosition, 0.1f); // Use a small radius
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall")) // Only check against walls
            {
                return false; // Cannot move into a wall
            }
        }
        return true; // Can move
    }

    private void DropItem()
    {
        // Stop dragging and reset the state
        isDragging = false;
        sortingManager.ItemDropped(this); // Notify manager that item has been dropped
    }
}
