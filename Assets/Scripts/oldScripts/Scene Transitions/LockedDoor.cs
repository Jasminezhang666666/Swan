using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door
{
    [SerializeField] string playerPrefKey;
    [SerializeField] protected Sprite unlockedDoor;

    [Header("Statuses")]
    [SerializeField] protected string lockedMsg = "It could lead to the theater stage, but it’s locked now.";
    [SerializeField] string itmMissMsg = "You still have unfinished business here.";

    [Header("Necessary Items")]
    [SerializeField] protected string keyObject;
    [SerializeField] string[] necessaryItems;

    protected bool locked = true;

    protected DialogueManager dialogueManager;

    protected override void Start()
    {
        base.Start();

        dialogueManager = manager.GetComponent<DialogueManager>();
        LoadLocked();
    }

    protected override void Update()
    {
        if (locked && Input.GetKeyDown(KeyCode.Return) && isInteracting && InventoryStorage.instance.selectedItemId == keyObject) // on key object use
        {
            if(!CheckForItems()) // necessary item check
            {
                // some items not found
                dialogueManager.AddDialogue(itmMissMsg);
            } else // all found
            {
                print("Open door!");
                InventoryStorage.instance.RemoveItem(keyObject); // remove key

                StoreLocked(false); // unlock door
                PlayerPrefs.SetFloat(doorPref, targetLoc); // set target location for player
                sceneTransition.SwitchScene(nextSceneName);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isInteracting) // on interact and locked
        {

            if (locked)
            {
                dialogueManager.AddDialogue(lockedMsg);
            } else if (!locked)
            {
                PlayerPrefs.SetFloat(doorPref, targetLoc); // set target location for player
                sceneTransition.SwitchScene(nextSceneName);
            }

        }
    }

    protected void StoreLocked(bool locked)
    {
        PlayerPrefs.SetInt(playerPrefKey, locked ? 1 : 0); // if locked, 1, if unlocked, 0
    }

    protected virtual void LoadLocked()
    {
        if (PlayerPrefs.HasKey(playerPrefKey)) // if locked door state already exists
        {
            locked = (PlayerPrefs.GetInt(playerPrefKey) == 1) ? true : false;
            Debug.Log("Door is unlocked");

            if (!locked)
            {
                highlightedPic = unlockedDoor;
            }
        }
    }

    private bool CheckForItems() // check for items needed
    {
        foreach (string item in necessaryItems)
        {
            if(!InventoryStorage.instance.HasItem(item))
            {
                return false; // necessary item not found
            }
        }

        return true; // necessary items found
    }
}
