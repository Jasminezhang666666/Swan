using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private Image image;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite originalSprite;

    private void Awake()
    {
        Deselect();
    }

    private void Start()
    {
        image = GetComponent<Image>(); 
        image.sprite = originalSprite; 
    }

    private void Select()
    {
        image.sprite = selectedSprite; // Change to selected sprite
    }
    private void Deselect()
    {
        image.sprite = originalSprite; // Revert to original sprite
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

}
