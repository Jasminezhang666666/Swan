using UnityEngine;
using System.Collections.Generic;

public class FanInventoryManager : MonoBehaviour
{
    public List<Transform> slotPositions;  // References to slot positions for visible items (3 slots)
    public List<FanItem> inventorySlots;   // List of FanItem components, each attached to a UI slot
    [SerializeField] private Transform storagePosition;  // Transform for storing other items
    [SerializeField] private Transform startPositionNearSlot0;  // Starting position for items near slot 0
    [SerializeField] private Transform startPositionNearSlot2;  // Starting position for items near slot 2
    [SerializeField] private float moveSpeed = 2.0f;  // Speed of the lerp movement

    private bool isLerping = false;  // Check if currently lerping
    private List<Vector3> targetPositions = new List<Vector3>();  // Target positions for lerping

    private void Start()
    {
        SetupTargetPositions();
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            RotateItems(-1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            RotateItems(1);
        }

        if (isLerping)
        {
            LerpItems();
        }
    }

    private void SetupTargetPositions()
    {
        targetPositions.Clear();
        int count = inventorySlots.Count;
        for (int i = 0; i < count; i++)
        {
            if (i < 3)  // First three items
            {
                targetPositions.Add(slotPositions[i].position);
            }
            else if (i == 3)  // Item next to the third slot
            {
                targetPositions.Add(startPositionNearSlot2.position);
            }
            else if (i == count - 1 && count > 3)  // Item next to the first slot
            {
                targetPositions.Add(startPositionNearSlot0.position);
            }
            else
            {
                targetPositions.Add(storagePosition.position);
            }
        }
        isLerping = true; // Enable lerping since we just set up new target positions
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
            SetupTargetPositions();  // Update target positions after rotation
        }
    }

    public void AddItem(FanItem item)
    {
        if (!inventorySlots.Contains(item))
        {
            inventorySlots.Insert(1, item);  // Add the item directly to the second slot position
            SetupTargetPositions();  // Update target positions
        }
    }

    public void RemoveItem(FanItem item)
    {
        if (inventorySlots.Contains(item))
        {
            inventorySlots.Remove(item);
            SetupTargetPositions();  // Update target positions
        }
    }
}
