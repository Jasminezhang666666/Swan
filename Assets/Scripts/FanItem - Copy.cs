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
                        FanInventoryManager.SetDraggingItem(true); // Set the isDraggingItem to true

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
