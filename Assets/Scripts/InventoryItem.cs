using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerDownHandler
{
    [Header("UI")]
    public Image image;

    [HideInInspector] public Item item;
    [HideInInspector] public static Transform parentAfterDrag;
    private static InventoryItem currentlyDraggedItem;

    private bool isDragging = false;
    private Canvas parentCanvas;
    private Camera mainCamera;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main; // Assuming the main camera is set correctly
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
        // Toggle dragging state
        if (!isDragging)
        {
            StartDragging();
        }
        else
        {
            StopDragging();
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
        parentAfterDrag = transform.parent;
        transform.SetParent(parentCanvas.transform, true); // Optional: consider if you want to change the parent
        isDragging = true;
        currentlyDraggedItem = this;
        image.raycastTarget = false;
    }

    private void StopDragging()
    {
        transform.SetParent(parentAfterDrag, true); // Set back to the original parent
        // Optional: Reset local position if necessary, e.g., transform.localPosition = Vector3.zero;
        isDragging = false;
        currentlyDraggedItem = null;
        image.raycastTarget = true;
    }

    private void FollowCursor()
    {
        if (!isDragging || parentCanvas.renderMode != RenderMode.WorldSpace) return;

        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Mathf.Abs(mainCamera.transform.position.z - parentCanvas.transform.position.z); // Adjust z based on canvas and camera position
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