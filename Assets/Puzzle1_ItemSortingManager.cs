using System.Collections.Generic;
using UnityEngine;

public class ItemSortingManager : MonoBehaviour
{
    private List<MainPuzzle1_item> items = new List<MainPuzzle1_item>();
    private int currentMaxSortingOrder = 0;

    public void RegisterItem(MainPuzzle1_item item)
    {
        items.Add(item);
    }

    public void ItemDropped(MainPuzzle1_item item)
    {
        // Increase the sorting order for the dropped item
        currentMaxSortingOrder++;
        if (currentMaxSortingOrder <= 1000)
        {
            item.SetSortingOrder(currentMaxSortingOrder);
        }
        else
        {
            Debug.LogWarning("Max sorting order reached. No more items can be brought to the front.");
        }
    }

    public void BringToFront(MainPuzzle1_item item)
    {
        // Set the clicked item to the highest sorting order
        item.SetSortingOrder(currentMaxSortingOrder);
    }
}
