using UnityEngine;
using Fungus;
using AK.Wwise;

public class TransparentDoor_Dressing02 : MonoBehaviour
{
    [Header("Requirement")]
    [Tooltip("Has the player fixed the floor?")]
    public bool FloorFixed = false;

    [Header("Fungus Blocks")]
    [Tooltip("Block telling the player to get a hammer")]
    public string needHammerBlock = "";

    [Tooltip("Block telling the player this isn't the right way")]
    public string wrongWayBlock = "";

    [Header("Fungus Flowchart")]
    [Tooltip("Flowchart containing the above blocks")]
    public Flowchart dialogueFlowchart;

    [Header("Audio (optional)")]
    [Tooltip("Door open / interaction sound")]
    public AK.Wwise.Event Door_Open;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // play door sound if assigned
        if (Door_Open != null)
            Door_Open.Post(gameObject);

        // ensure we have a flowchart
        if (dialogueFlowchart == null)
        {
            Debug.LogWarning("TransparentDoor_Dressing02: no Flowchart assigned.");
            return;
        }

        // choose which Fungus block to run
        if (!FloorFixed)
        {
            if (!string.IsNullOrEmpty(needHammerBlock))
                dialogueFlowchart.ExecuteBlock(needHammerBlock);
            else
                Debug.LogWarning("TransparentDoor_Dressing02: needHammerBlock is empty.");
        }
        else
        {
            if (!string.IsNullOrEmpty(wrongWayBlock))
                dialogueFlowchart.ExecuteBlock(wrongWayBlock);
            else
                Debug.LogWarning("TransparentDoor_Dressing02: wrongWayBlock is empty.");
        }
    }
}
