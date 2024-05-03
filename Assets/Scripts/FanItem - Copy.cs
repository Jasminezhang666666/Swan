using UnityEngine;
using UnityEngine.EventSystems; // Required namespace for Event Triggers
using UnityEngine.UI;

public class FanItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image itemImage; // The UI Image component that displays the item's sprite
    public Item itemData; // The item data this FanItem represents

    [SerializeField] private Sprite highlightSprite; // Sprite to use when item is highlighted

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

    public void SetHighlightSprite(Sprite sprite)
    {
        highlightSprite = sprite;
    }

    private void UpdateItemUI()
    {
        if (itemImage != null && itemData != null)
        {
            itemImage.sprite = itemData.image;
        }
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
        else
            Debug.LogError("Item data or image is missing on " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle click event if needed
    }
}
