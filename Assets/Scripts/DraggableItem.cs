using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public Item itemData; // Store item data
    private FanItem originalFanItem; // Reference to the original FanItem

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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the item stays on the same plane
        transform.position = mousePosition;

        if (Input.GetMouseButtonDown(1)) // Check if the right mouse button is pressed
        {
            FanInventoryManager.SetDraggingItem(false); // Set the isDraggingItem to false

            // Re-activate the original FanItem and add it back to the inventory
            if (originalFanItem != null)
            {
                originalFanItem.gameObject.SetActive(true); // Reactivate the FanItem
                FanInventoryManager fanInventoryManager = FindObjectOfType<FanInventoryManager>();
                if (fanInventoryManager != null)
                {
                    fanInventoryManager.AddItem(originalFanItem); // Add the FanItem back to the inventory
                }
                else
                {
                    Debug.LogError("FanInventoryManager not found in the scene.");
                }
            }

            Destroy(gameObject); // Destroy the DraggableItem
        }
    }
}
