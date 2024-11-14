using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FanInventoryManager : MonoBehaviour
{
    public List<Transform> slotPositions;  // References to slot positions for visible items
    public List<FanItem> inventorySlots;   // List of FanItem components

    public static bool isDraggingItem = false;     // Global bool to track if a DraggableItem is being dragged

    [SerializeField] private Transform storagePosition;  // Transform for storing other items
    [SerializeField] private Transform startPositionNearSlot0;  // Starting position near slot 0
    [SerializeField] private Transform startPositionNearSlot2;  // Starting position near slot 2
    [SerializeField] private GameObject inventoryPanel;  // The panel that contains the inventory UI
    [SerializeField] private Button closeButton;  // Button to close the inventory
    [SerializeField] private Image Pattern; //Pattern inside the inventory

    [SerializeField] private float moveSpeed = 2.0f;  // Speed of the lerp movement
    [SerializeField] private float scrollCooldown = 0.2f;  // Cooldown time between scrolls in seconds
    [SerializeField] private float rotationSpeed = -20.0f; //Speed of the image rotation

    private bool isLerping = false;  // Check if currently lerping
    private List<Vector3> targetPositions = new List<Vector3>();  // Target positions for lerping
    private float lastScrollTime = 0f;  // Last time the scroll was activated

    private void Start()
    {
        SetupTargetPositions();
        inventoryPanel.SetActive(false);  // Start with the inventory hidden

        // Ensure the close button is setup and has a listener
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
        else
        {
            Debug.LogError("Close button not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (Time.time - lastScrollTime > scrollCooldown)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                RotateItems(-1);
                RotateImage(-1);
                lastScrollTime = Time.time;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                RotateItems(1);
                RotateImage(1);
                lastScrollTime = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryVisibility();
        }

        if (isLerping)
        {
            LerpItems();
        }
        
    }


    private void ToggleInventoryVisibility()
    {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);
        closeButton.gameObject.SetActive(!isActive); // Show close button when inventory is shown
    }

    public void CloseInventory()  // Method to close the inventory
    {
        inventoryPanel.SetActive(false);
    }

    private void SetupTargetPositions()
    {
        targetPositions.Clear();
        int count = inventorySlots.Count;
        for (int i = 0; i < count; i++)
        {
            if (i < 3)
            {
                targetPositions.Add(slotPositions[i].position);
            }
            else if (i == 3)
            {
                targetPositions.Add(startPositionNearSlot2.position);
            }
            else if (i == count - 1 && count > 3)
            {
                targetPositions.Add(startPositionNearSlot0.position);
            }
            else
            {
                targetPositions.Add(storagePosition.position);
            }
        }
        isLerping = true;
    }

    private void LerpItems()
    {
        bool stillLerping = false;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (Vector3.Distance(inventorySlots[i].transform.position, targetPositions[i]) > 0.01f)
            {
                inventorySlots[i].transform.position = Vector3.Lerp(inventorySlots[i].transform.position, targetPositions[i], Time.deltaTime * moveSpeed);
                stillLerping = true;
            }
            else
            {
                inventorySlots[i].transform.position = targetPositions[i];
            }
        }
        isLerping = stillLerping;
    }

    private void RotateItems(int direction)
    {
        if (inventorySlots.Count > 1)
        {
            if (direction > 0)
            {
                FanItem temp = inventorySlots[inventorySlots.Count - 1];
                inventorySlots.RemoveAt(inventorySlots.Count - 1);
                inventorySlots.Insert(0, temp);
            }
            else
            {
                FanItem temp = inventorySlots[0];
                inventorySlots.RemoveAt(0);
                inventorySlots.Add(temp);
            }
            SetupTargetPositions();
        }
    }

    public void AddItem(FanItem item)
    {
        if (!inventorySlots.Contains(item))
        {
            inventorySlots.Insert(1, item);
            SetupTargetPositions();
        }
    }

    public void RemoveItem(FanItem item)
    {
        if (inventorySlots.Contains(item))
        {
            inventorySlots.Remove(item);
            item.gameObject.SetActive(false); // Deactivate the item instead of destroying it
            SetupTargetPositions(); // Update positions of remaining items
        }
    }

    private void RotateImage(int direction)
    {
        float rotationAmount = direction * rotationSpeed;
     
                    
            Vector3 center = Pattern.transform.position;
            Pattern.transform.RotateAround(center, Vector3.back, rotationAmount*2);
       
    }
    public static void SetDraggingItem(bool isDragging)
    {
        isDraggingItem = isDragging;
    }

    public static bool GetDraggingItem()
    {
        return isDraggingItem;
    }
}