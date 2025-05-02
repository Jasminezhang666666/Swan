using UnityEngine;
using Fungus;

public class MainPuzzle1_itemFinal : MainPuzzle1_item
{
    [Header("Room Swap Objects")]
    public GameObject dressingRoom;
    public GameObject puzzle1;

    [Header("Other Objects to Toggle")]
    [Tooltip("The bag object in Puzzle 1 whose BoxCollider2D should be disabled")]
    public GameObject puzzle1Bag;

    [Header("Fungus Flowchart")]
    [Tooltip("Drag in the Flowchart that contains the 'NeedToChange' block")]
    public Flowchart flowchart;

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

        // hide this item visually and disable clicks
        if (TryGetComponent<Collider>(out Collider col))
            col.enabled = false;
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            sr.enabled = false;

        // disable the bag's collider so it can't be clicked
        if (puzzle1Bag != null)
        {
            var box2D = puzzle1Bag.GetComponent<BoxCollider2D>();
            if (box2D != null)
                box2D.enabled = false;
        }

        // mark all doors as having changed cloth
        var allDoors = Resources.FindObjectsOfTypeAll<E_Door_MultipleOutput>();
        foreach (var door in allDoors)
        {
            door.ChangedCloth = true;
        }

        // add this item to inventory
        /*
        var newItem = GetComponent<NewItem>();
        if (newItem != null)
            newItem.GoToInventory();
        */

        // swap rooms
        print("CHANGE ROOOOOOM");
        dressingRoom?.SetActive(true);
        puzzle1?.SetActive(false);

        // fire your Fungus block
        if (flowchart != null)
            flowchart.ExecuteBlock("NeedToChange");
    }
}
