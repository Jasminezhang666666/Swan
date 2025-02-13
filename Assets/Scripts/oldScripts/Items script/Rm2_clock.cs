using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Rm2_clock : Items
{
    [Header("Sprites")]
    [SerializeField] Sprite openState;
    [SerializeField] Sprite withGear; // gear inside
    [SerializeField] Sprite closedClock; // closed
    [SerializeField] Sprite closedHighlight; // highlighted while closed
    [SerializeField] Sprite hammerDropped; // hammer dropped
    [SerializeField] Sprite hammerHighlight; // hammer highlighted
    [SerializeField] Sprite finalState;

    [Header("Settings")]
    [SerializeField] string gearName = "Gear";
    string hammerName = "Hammer";

    [SerializeField] AudioSource source;

    int stage = 0;
    string clockPref = "ClockStage";

    // for hammer animation
    Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>(); // get animator

        if (PlayerPrefs.HasKey(clockPref)) // if clock state already exists
        {
            stage = PlayerPrefs.GetInt(clockPref); // load saved state
            SetStage();
        }
    }

    protected override void Interact() // clock interactions
    {
        isInteracting = true;
        Debug.Log("clock stage: " + stage);
        
        // spaghetti code...
        if(stage == 0) // if it is base stage, set to 0
        {
            ActivateStage(stage);
        } else if (stage == 1) // if it is stage 1, move to final stage
        {
            stage++;
            ActivateStage(stage);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (stage == 0 && onObject && InventoryStorage.instance.selectedItemId == gearName) // on gear used
        {
            print("Clock Changed");
            InventoryStorage.instance.RemoveItem(gearName); // remove gear

            stage = 1; // stage 1
            ActivateStage(stage); // activate stage 1
        }
    }

    private void SetStage()
    {
        switch (stage)
        {
            case 0: // nothing has happened yet
                SetOgAndHighlight(closedClock, closedHighlight);
                break;
            case 1:
                // gear used
                SetOgAndHighlight(hammerDropped, hammerHighlight);
                break;
            case 2:
                // hammer taken
                SetOgAndHighlight(finalState, finalState);
                break;
            default:
                break;
        }
    }

    private void ActivateStage(int num)
    {
        //InventoryStorage.instance.player.canMove = false; // stop player movement
        switch (num)
        {
            case 0: // show interaction with clock
                manager.GetComponent<DialogueManager>().AddDialogue("An old tall mechanical clock.");
                manager.GetComponent<DialogueManager>().AddDialogue("One of the gears seems to be missing.");
                break;
            case 1: // gear used
                anim.enabled = true; // enable animator
                anim.SetTrigger("Drop"); // show animation of dropping hammer

                //play sound
                source.Play();

                float delay = 3f;

                // new hover states
                StartCoroutine(WaitToSwap(delay, hammerDropped));
                SetOgAndHighlight(hammerDropped, hammerHighlight);
                break;
            case 2: // hammer picked up

                if (!InventoryStorage.instance.HasItem(hammerName))
                {
                    InventoryStorage.instance.AddItem(hammerName);
                }

                SetOgAndHighlight(finalState, finalState);
                break;
            default:
                break;
        }
        //InventoryStorage.instance.player.canMove = true; // allow player movement
    }

    private void SetOgAndHighlight(Sprite og, Sprite highlighted)
    {
        originalPic = og;
        highlightedPic = highlighted;
        rend.sprite = originalPic;
    }

    public void DisableAnim() // disables animator at the end of the animation
    {
        anim.enabled = false;
    }

    IEnumerator WaitToSwap(float time, Sprite swap)
    {
        yield return new WaitForSeconds(time);
        rend.sprite = swap;
        anim.enabled = false;
    }
}
