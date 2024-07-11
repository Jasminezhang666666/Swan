using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public Item itemData; // Store item data

    // Method to set the item data from the FanItem
    public void SetItemData(Item data)
    {
        itemData = data;
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
    }
}
