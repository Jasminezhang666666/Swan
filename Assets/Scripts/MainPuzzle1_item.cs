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
    private ParticleSystem particleSystem;

    protected void Start()
    {
        
        particleSystem = GameObject.FindGameObjectWithTag("ClickRipple").GetComponent<ParticleSystem>();
        particleSystem.Pause();
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
        
        //hard code the particle system sorting order
        ParticleSystemRenderer psRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        psRenderer.sortingLayerName = "Default"; 
        psRenderer.sortingOrder = 102; 

    }
    

    
    // protected void Update()
    // {
    //     if (isDragging)
    //     {
    //         Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //         Vector3 targetPosition = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
    //         Vector3 newPosition = transform.position;
    //
    //         // Check if the full movement is valid
    //         if (CanMoveTo(targetPosition))
    //         {
    //             newPosition = targetPosition;
    //         }
    //         else
    //         {
    //             // Try horizontal movement only
    //             Vector3 horizontalMove = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
    //             if (CanMoveTo(horizontalMove))
    //             {
    //                 newPosition.x = horizontalMove.x;
    //             }
    //         
    //             // Try vertical movement only
    //             Vector3 verticalMove = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
    //             if (CanMoveTo(verticalMove))
    //             {
    //                 newPosition.y = verticalMove.y;
    //             }
    //         }
    //     
    //         rb.MovePosition(newPosition);
    //     }
    // }
    
    protected void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure z is set to 0
            Vector3 targetPosition = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, 0); // Ensure z is set to 0
            Vector3 newPosition = transform.position;

            // Define the boundaries
            float minX = -10f; // Set your minimum X boundary
            float maxX = 10f;  // Set your maximum X boundary
            float minY = -10f;  // Set your minimum Y boundary
            float maxY = 10f;   // Set your maximum Y boundary

            // Check if the full movement is valid
            if (CanMoveTo(targetPosition) && IsWithinBounds(targetPosition, minX, maxX, minY, maxY))
            {
                newPosition = targetPosition;
            }
            else
            {
                // Try horizontal movement only
                Vector3 horizontalMove = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
                if (CanMoveTo(horizontalMove) && IsWithinBounds(horizontalMove, minX, maxX, minY, maxY))
                {
                    newPosition.x = horizontalMove.x;
                }

                // Try vertical movement only
                Vector3 verticalMove = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                if (CanMoveTo(verticalMove) && IsWithinBounds(verticalMove, minX, maxX, minY, maxY))
                {
                    newPosition.y = verticalMove.y;
                }
            }

            rb.MovePosition(newPosition);

            // Update particle system position to follow the mouse
            particleSystem.transform.position = mousePosition;
        }

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
        // Use the size of your current collider bounds
        Vector2 size = myCollider.bounds.size;
    
        // Check for colliders overlapping the box at the target position
        Collider2D[] colliders = Physics2D.OverlapBoxAll(targetPosition, size, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return false; // Cannot move into a wall
            }
        }
        return true; // Movement is allowed
    }

    

    
    private void DropItem()
    {
        // Stop dragging and reset the state
        isDragging = false;
        sortingManager.ItemDropped(this); // Notify manager that item has been dropped
    }
    
    private bool IsWithinBounds(Vector3 position, float minX, float maxX, float minY, float maxY)
    {
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }
    
    protected virtual void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            particleSystem.Play();
            offset = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            sortingManager.BringToFront(this); // Bring this item to front immediately
        }
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;
        particleSystem.Stop();
        // Notify manager that item has been dropped, even if it's dropped because of a wall
        sortingManager.ItemDropped(this);
    }
}
