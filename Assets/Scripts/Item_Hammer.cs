using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item_Hammer : InventoryItem
{
    public static bool HammerDragging = false;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData); // Call the base method to ensure the base functionality is preserved
        HammerDragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData); // Call the base method to ensure the base functionality is preserved
        HammerDragging = false;
    }
}
