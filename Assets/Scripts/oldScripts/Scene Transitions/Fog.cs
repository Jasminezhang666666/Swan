using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : LockedDoor
{
    [Header("Blocking Settings")]
    [SerializeField] float minBoundWhenBlocked = 4f;
    [SerializeField] Sprite unlockedSprite = null;
    Player player;
    float playerInitialMin;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerInitialMin = player.xMinBound;
        
        if (locked) // if this thing is locked
        {
            // block the player's way
            player.xMinBound = minBoundWhenBlocked;
            Debug.Log("Fog locked");
        } else
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        GetComponent<SpriteRenderer>().sprite = unlockedSprite; // unlocked
        highlightedPic = unlockedSprite;
        originalPic = unlockedSprite;
        unlockedDoor = unlockedSprite;
        player.xMinBound = playerInitialMin; // allow player movement
        locked = false;
        StoreLocked(locked);
    }

    protected override void LoadLocked()
    {
        base.LoadLocked();

        if (!locked)
        {
            Unlock();
        }
    }

    protected override void Update() // NO SCENE TRANSITIONS!
    {
        if (locked && Input.GetKeyDown(KeyCode.Return) && isInteracting && InventoryStorage.instance.selectedItemId == keyObject) // on key object use
        {
            print("Unlocked!");
            InventoryStorage.instance.RemoveItem(keyObject); // remove key

            StartCoroutine(PlayAnimation(1f));
            StoreLocked(false); // unlock door
        }

        if (Input.GetKeyDown(KeyCode.E) && isInteracting) // on interact and locked
        {
            if (locked)
            {
                dialogueManager.AddDialogue(lockedMsg);
            }
            else if (!locked)
            {
                player.xMinBound = playerInitialMin; // reset player minbound
            }
        }
    }

    IEnumerator PlayAnimation(float time)
    {
        anim.enabled = true;
        yield return new WaitForSeconds(time);
        anim.enabled = false;
        Unlock();
    }
}
