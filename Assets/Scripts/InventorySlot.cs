using System.Collections;
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
