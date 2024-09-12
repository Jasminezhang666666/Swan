using UnityEngine;
using UnityEngine.EventSystems; // Required namespace for Event Triggers
using UnityEngine.UI;

public class FanItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image itemImage; // The UI Image component that displays the item's sprite
    public Item itemData; // The item data this FanItem represents

    [SerializeField] private Sprite highlightSprite; // Sprite to use when item is highlighted
    [SerializeField] private GameObject draggablePrefab; // Prefab for the draggable item

    void Awake()
    {
        if (itemImage == null)
            itemImage = GetComponent<Image>();
    }

    public void SetupItem(Item newItemData)
    {
        itemData = newItemData;
        UpdateItemUI();
    }

    private void UpdateItemUI()
    {
        if (itemImage != null && itemData != null)
        {
            itemImage.sprite = itemData.image;
        }
    }

    public void SetHighlightSprite(Sprite sprite)
    {
        highlightSprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightSprite != null)
            itemImage.sprite = highlightSprite; // Change to highlight sprite on hover
        else
            Debug.LogError("Highlight sprite not assigned on " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemData != null && itemData.image != null)
            itemImage.sprite = itemData.image; // Revert to normal sprite
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // Check if the left mouse button was clicked
        {
            FanInventoryManager fanInventoryManager = FindObjectOfType<FanInventoryManager>();
            if (fanInventoryManager != null)
            {
                // If another item is being dragged, replace it with this item
                if (FanInventoryManager.GetDraggingItem())
                {
                    DraggableItem currentDraggableItem = FindObjectOfType<DraggableItem>();
                    if (currentDraggableItem != null)
                    {
                        // Reactivate and return the currently dragged item to the inventory
                        FanItem originalFanItem = currentDraggableItem.GetOriginalFanItem();
                        if (originalFanItem != null)
                        {
                            originalFanItem.gameObject.SetActive(true); // Reactivate the original FanItem
                            fanInventoryManager.AddItem(originalFanItem); // Add the original FanItem back to the inventory
                        }

                        Destroy(currentDraggableItem.gameObject); // Destroy the currently dragged DraggableItem
                    }
                }

                fanInventoryManager.RemoveItem(this); // Remove this FanItem from the inventory
                fanInventoryManager.CloseInventory(); // Close the inventory panel

                if (draggablePrefab != null)
                {
                    // Create instance with prefab's rotation and position based on mouse position
                    GameObject draggableItem = Instantiate(draggablePrefab, Input.mousePosition, draggablePrefab.transform.rotation);

                    // Optionally set scale to match prefab's original scale
                    draggableItem.transform.localScale = draggablePrefab.transform.localScale;

                    DraggableItem dragScript = draggableItem.GetComponent<DraggableItem>();
                    if (dragScript != null)
                    {
                        dragScript.SetItemData(itemData, this); // Pass the item data and original FanItem reference

                        // Add DraggableItemHandler to manage the movement
                        DraggableItemHandler handler = draggableItem.GetComponent<DraggableItemHandler>();
                        if (handler != null)
                        {
                            handler.wheel = FindObjectOfType<Puzzle4_wheel>().transform; // Assign the wheel
                        }

                        FanInventoryManager.SetDraggingItem(true); // Keep the isDraggingItem set to true

                        gameObject.SetActive(false); // Deactivate the original FanItem
                    }
                    else
                    {
                        Debug.LogError("DraggableItem script not found on prefab.");
                    }
                }
                else
                {
                    Debug.LogError("Draggable prefab is not assigned.");
                }
            }
            else
            {
                Debug.LogError("FanInventoryManager not found in the scene.");
            }
        }
    }

}
