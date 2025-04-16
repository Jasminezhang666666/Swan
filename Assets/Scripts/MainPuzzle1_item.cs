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
    private Rigidbody2D rb;
    private ItemSortingManager sortingManager;
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
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found. Please add one.");
        }
        else
        {
            rb.isKinematic = true; // Ensure it is kinematic for dragging
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        sortingManager = FindObjectOfType<ItemSortingManager>();
        sortingManager.RegisterItem(this);

        // Hard code the particle system sorting order
        ParticleSystemRenderer psRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        psRenderer.sortingLayerName = "Default";
        psRenderer.sortingOrder = 102;
    }

    protected void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure z is 0

            // Calculate target position based on mouse + offset
            Vector3 desiredPosition = mousePosition + offset;
            Vector3 newPosition = transform.position;

            // Check if moving fully is allowed
            if (CanMoveTo(desiredPosition))
            {
                newPosition = desiredPosition;
            }
            else
            {
                // Try moving horizontally only
                Vector3 horizontalMove = new Vector3(desiredPosition.x, transform.position.y, 0);
                if (CanMoveTo(horizontalMove))
                {
                    newPosition.x = desiredPosition.x;
                }

                // Try moving vertically only
                Vector3 verticalMove = new Vector3(transform.position.x, desiredPosition.y, 0);
                if (CanMoveTo(verticalMove))
                {
                    newPosition.y = desiredPosition.y;
                }
            }

            //rb.MovePosition(newPosition);
            transform.position = newPosition;


            // Recalculate offset for smooth dragging
            offset = transform.position - mousePosition;

            // Move particle system effect with mouse
            particleSystem.transform.position = mousePosition;
        }
    }

    private bool CanMoveTo(Vector3 targetPosition)
    {
        Vector2 size = myCollider.bounds.size;

        // Check if moving to target position would hit any Wall
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPosition, size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Wall"))
            {
                return false; // Hit a forbidden wall
            }
        }
        return true; // No walls hit, allow move
    }

    public void SetSortingOrder(int order)
    {
        if (myRenderer != null)
        {
            myRenderer.sortingOrder = order;
        }
    }

    private void DropItem()
    {
        // Stop dragging and reset the state
        isDragging = false;
        sortingManager.ItemDropped(this); // Notify manager that item has been dropped
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
        sortingManager.ItemDropped(this); // Notify manager that item has been dropped
    }
}
