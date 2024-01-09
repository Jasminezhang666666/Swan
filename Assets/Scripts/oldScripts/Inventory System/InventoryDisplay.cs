using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InvDatabase;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryDisplay : MonoBehaviour
{
    // dialogue
    DialogueManager dialogueManager;

    // inventory bar display
    [SerializeField] GameObject invSlot;
    public static int NUMBER_OF_SLOTS = 8; // total number of inventory slots
    GameObject[] invSlots = new GameObject[NUMBER_OF_SLOTS];

    Canvas canvas; // canvas for toggling

    // for cycling through items
    bool inventorySelection = false;
    int selectedBoxNum = 0;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        for (int i = 0; i < NUMBER_OF_SLOTS; i++)
        {
            invSlots[i] = Instantiate(invSlot, transform); // add slot to inventory (this)
            Debug.Log(i);
        }
    }

    public void Refresh() // refresh inventory to show items
    {
        // clear all parts
        for (int i = 0; i < NUMBER_OF_SLOTS; i++)
        {
            Image itemUI = invSlots[i].transform.GetChild(0).GetComponent<Image>(); // get child of inv slot
            itemUI.enabled = false;
        }

        // show items that exist
        for (int i = 0; i < InventoryStorage.instance.Size(); i++)
        {
            Image itemUI = invSlots[i].transform.GetChild(0).GetComponent<Image>(); // get first child of inv slot
            Sprite itemSprite = InventoryStorage.instance.At(i).invSprite; // store item sprite from that item

            itemUI.sprite = itemSprite; // set item sprite
            itemUI.enabled = true; // enable the sprite
        }
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
    }

    private void Update()
    {
        if (!dialogueManager)
        {
            GameObject manager = GameObject.FindGameObjectWithTag("Manager");
            if (manager)
            {
                dialogueManager = manager.GetComponent<DialogueManager>();
            }
        }

        //if press E, open/close inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventorySelection) //close inventory
            {
                CloseInventorySelect();

            }
            else //open inventory
            {
                OpenInventorySelect();

            }
        }

        //if the inventory is opened
        if (inventorySelection)
        {
            //change selection use [A] or [D]
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && selectedBoxNum > 0)
            {
                invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
                selectedBoxNum--;
                invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && selectedBoxNum < invSlots.Length - 1)
            {
                invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
                selectedBoxNum++;
                invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
            }

            //If [Enter]
            if (Input.GetKeyDown(KeyCode.Return) && canvas.enabled)
            {
                Hide();
                if (selectedBoxNum < InventoryStorage.instance.Size())
                {
                    Itm selectedItem = InventoryStorage.instance.At(selectedBoxNum);
                    InventoryStorage.instance.selectedItemId = selectedItem.id;

                    string description = InventoryStorage.instance.At(selectedBoxNum).description;
                    dialogueManager.AddDialogue(description);
                    Debug.Log(InventoryStorage.instance.selectedItemId + " added to inventory.");
                }
                else
                {
                    InventoryStorage.instance.selectedItemId = null;
                    dialogueManager.AddDialogue("Empty Slot.");
                }

                CloseInventorySelect();
            }
        }
    }


    private void CloseInventorySelect() //close selection
    {
        print("Inventory selection closed.");
        inventorySelection = false;

        //deselect the current selected boxes
        invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
        //player can move again
        InventoryStorage.instance.player.canMove = true;
    }

    private void OpenInventorySelect() //open selection
    {
        print("Inventory selection opened.");
        inventorySelection = true;

        selectedBoxNum = 0;
        invSlots[selectedBoxNum].GetComponent<ItemBox>().ChangeSelect();
        //player can't move anoymore
        InventoryStorage.instance.player.canMove = false;

        Show();
    }

}
