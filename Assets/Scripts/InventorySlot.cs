using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite originalSprite;

    public bool selected;
    public bool isHoveredOver = false; // New flag to check if an item is hovering over this slot

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = originalSprite;
        Deselect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
        isHoveredOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
        isHoveredOver = false;
    }

    public void Select()
    {
        image.sprite = selectedSprite; // Change to selected sprite
        selected = true;
    }

    public void Deselect()
    {
        image.sprite = originalSprite; // Revert to original sprite
        selected = false;
    }

    // Attempt to place an item into this slot
    public bool TryPlaceItem(InventoryItem item)
    {
        if (transform.childCount == 0) // The slot is empty, can place the item
        {
            item.transform.SetParent(transform, false);
            item.transform.localPosition = Vector3.zero; // Center item in the slot
            item.StopDragging(); // Ensure the item stops dragging
            return true;
        }
        return false; // Slot was not empty, item placement failed
    }


}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Image image;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite originalSprite;

    public bool selected;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = originalSprite;
        Deselect();
    }

    //鼠标选择: 
    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }
    public void Select()
    {
        image.sprite = selectedSprite; // Change to selected sprite
        selected = true;
    }
    public void Deselect()
    {
        image.sprite = originalSprite; // Revert to original sprite
        selected = false;
    }

    
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) //can be placed only when there's no other chihld
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
            InventoryItem.parentAfterDrag = transform;
        }

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
        if (selected)
        {
            //这里被选择，想做什么就做什么？
            

        }
    }

    private void DeleteItem()
    {
        InventoryItem itemInSlot = GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Destroy(itemInSlot.gameObject);
        }
    }

}
*/