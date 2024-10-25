using System.Collections.Generic;
using UnityEngine;

public class ItemSortingManager : MonoBehaviour
{
    private List<MainPuzzle1_item> items = new List<MainPuzzle1_item>();

    public void RegisterItem(MainPuzzle1_item item)
    {
        items.Add(item);
    }

    public void ItemDropped(MainPuzzle1_item item)
    {
        // Increase the sorting order for the dropped item
        int maxSortingOrder = GetMaxSortingOrder();
        item.SetSortingOrder(maxSortingOrder + 1); // Set to higher than current max
    }

    public void BringToFront(MainPuzzle1_item item)
    {
        // Set the clicked item to the highest sorting order
        int maxSortingOrder = GetMaxSortingOrder();
        item.SetSortingOrder(maxSortingOrder + 1); // Assign higher sorting order
    }

    private int GetMaxSortingOrder()
    {
        int maxOrder = 0;
        foreach (var item in items)
        {
            // Check if the item is tagged as "PuzzleItem" and get its sorting order
            if (item.CompareTag("PuzzleItem"))
            {
                maxOrder = Mathf.Max(maxOrder, item.GetComponent<SpriteRenderer>().sortingOrder);
            }
        }
        return maxOrder;
    }
}
