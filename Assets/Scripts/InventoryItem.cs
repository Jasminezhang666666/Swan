using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;

    [HideInInspector] public Item item;
    [HideInInspector] public static Transform parentAfterDrag;

    public void InitializeItem(Item newItem)
    {
        Debug.Log($"Initializing item with sprite: {newItem.image.name}");

        item = newItem;

        if(image == null)
        {
            image = GetComponent<Image>();
        }

        image.sprite = newItem.image;
    }


    //Drag and Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = Camera.main;
        Vector3 screenPoint = Input.mousePosition; // Get the current mouse position

        // Calculate the distance from the camera to the Canvas
        float distanceToCanvas = Vector3.Distance(transform.position, cam.transform.position);

        // Convert the screen point to a world point on the Canvas plane
        screenPoint.z = distanceToCanvas; // Set the z distance
        Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);

        // Move the UI element to the calculated world position
        transform.position = worldPoint;
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
