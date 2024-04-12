using UnityEngine;
using System.Collections.Generic;

public class FanInventoryManager : MonoBehaviour
{
    public List<Transform> slotPositions;  // References to empty GameObjects as slot positions for visible items
    public List<FanItem> inventorySlots;   // List of FanItem components, each attached to a UI slot
    [SerializeField] private Transform storagePosition;  // Transform for storing other items

    private void Start()
    {
        UpdateItemPositions();  // Position items according to the empty GameObjects
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            RotateItems(-1);  // Rotate left
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            RotateItems(1);   // Rotate right
        }
    }

    // Update the positions of each inventory item based on slot positions
    private void UpdateItemPositions()
    {
        int itemCount = inventorySlots.Count;
        int slotCount = slotPositions.Count;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (itemCount == 1 && i == 0)
            {
                // Position the single item in the second slot (center slot)
                inventorySlots[i].transform.position = slotPositions[Mathf.Clamp(1, 0, slotCount - 1)].position;
            }
            else if (i < slotCount)
            {
                inventorySlots[i].transform.position = slotPositions[i].position;
            }
            else
            {
                inventorySlots[i].transform.position = storagePosition.position; // Move excess items to the storage position
            }
        }
    }

    // Rotate the items in the inventory
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
            UpdateItemPositions();  // Update positions after rotation
        }
    }

    public void AddItem(FanItem item)
    {
        if (!inventorySlots.Contains(item))
        {
            inventorySlots.Add(item);
            UpdateItemPositions();  // Update positions to accommodate the new item
        }
    }

    public void RemoveItem(FanItem item)
    {
        if (inventorySlots.Contains(item))
        {
            inventorySlots.Remove(item);
            UpdateItemPositions();  // Update positions after removing the item
        }
    }
}
