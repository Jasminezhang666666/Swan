using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public Item itemData; // Store item data
    private FanItem originalFanItem; // Reference to the original FanItem
    private Transform wheel; // Reference to the wheel's transform
    public float proximityThreshold = 1f; // Distance within which the item will move to the wheel
    public float moveSpeed = 2f; // Speed at which the item will move to the wheel

    private bool isMovingToWheel = false; // Flag to indicate if the item is moving to the wheel

    void Start()
    {
        // Find the wheel in the scene
        wheel = FindObjectOfType<Puzzle4_wheel>()?.transform;

        if (wheel == null)
        {
            Debug.LogError("Puzzle4_wheel not found in the scene.");
        }
    }

    // Method to set the item data from the FanItem and reference to the original FanItem
    public void SetItemData(Item data, FanItem originalItem)
    {
        itemData = data;
        originalFanItem = originalItem;
        UpdateItemAppearance(); // Update the appearance based on the item data
    }

    private void UpdateItemAppearance()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && itemData != null)
        {
            spriteRenderer.sprite = itemData.image; // Assuming itemData contains a reference to the sprite
        }
    }
    private void Update()
    {
        // If the item is moving to the wheel
        if (isMovingToWheel)
        {
            // Move the draggable item towards the wheel's position
            Vector3 targetPosition = wheel.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Ensure the draggable item is exactly at the target position
                transform.position = targetPosition;

                // Make the draggable item a child of the wheel
                transform.SetParent(wheel);

                Debug.Log($"{gameObject.name} has become a child of the wheel and moved to position {targetPosition}.");

                // Stop dragging and stop the movement
                FanInventoryManager.SetDraggingItem(false);
                isMovingToWheel = false;

                // Disable this component or stop updating the item position
                this.enabled = false;
            }
        }
        else
        {
            // Continue following the mouse only if it's not moving to the wheel
            if (FanInventoryManager.GetDraggingItem())
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Ensure the item stays on the same plane
                transform.position = mousePosition;
            }

            if (Input.GetMouseButtonDown(0)) // Check if the left mouse button is pressed
            {
                Debug.Log($"Draggable Item Position: {transform.position}");

                if (wheel != null && Vector3.Distance(transform.position, wheel.position) < proximityThreshold)
                {
                    isMovingToWheel = true;
                }
            }
        }
    }

    // Method to get the original FanItem
    public FanItem GetOriginalFanItem()
    {
        return originalFanItem;
    }
}
