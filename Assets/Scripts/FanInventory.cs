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
        // Ensure only the first three items (or less, if fewer items exist) are displayed in the slots
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < slotPositions.Count)
            {
                inventorySlots[i].transform.position = slotPositions[i].position;
            }
            else
            {
                inventorySlots[i].transform.position = storagePosition.position; // Move other items to the storage position
            }
        }
    }

    // Rotate the items in the inventory
    private void RotateItems(int direction)
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
