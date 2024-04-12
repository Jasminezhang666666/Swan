using UnityEngine;
using UnityEngine.UI;

public class FanItem : MonoBehaviour
{
    public Image itemImage;  // The UI Image component that displays the item's sprite
    public Item itemData;    // The item data this FanItem represents

    // Initialize or update the item data and UI
    public void SetupItem(Item newItemData)
    {
        itemData = newItemData;
        UpdateItemUI();
    }

    private void UpdateItemUI()
    {
        if (itemImage != null && itemData != null)
        {
            itemImage.sprite = itemData.image;  // Use the 'image' property instead of 'sprite'
        }
    }
}
