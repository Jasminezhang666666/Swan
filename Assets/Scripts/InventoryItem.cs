using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler
{
    [Header("UI")]
    public Image image;

    [HideInInspector] public Item item;
    private Canvas parentCanvas;
    private Camera mainCamera;
    private bool isDragging = false;

    public static InventoryItem currentlyDraggedItem;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Attempt to drop the item into a new slot (if over one) or return it to its original slot
            StopDragging();
        }
        else
        {
            // Start dragging this item
            StartDragging();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            FollowCursor();
        }
    }

    private void StartDragging()
    {
        currentlyDraggedItem = this;
        isDragging = true;
        // Optionally, lift the item slightly above the rest or change its appearance
    }

    public void StopDragging()
    {
        currentlyDraggedItem = null;
        isDragging = false;
        // Reset any visual changes made when started dragging

        // The actual parent reassignment should be handled in response to a successful drop event or similar
    }

    private void FollowCursor()
    {
        if (!isDragging || parentCanvas.renderMode != RenderMode.WorldSpace) return;

        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Mathf.Abs(mainCamera.transform.position.z - parentCanvas.transform.position.z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        transform.position = worldPoint;
    }


}


/*
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
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
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



    public virtual void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
*/