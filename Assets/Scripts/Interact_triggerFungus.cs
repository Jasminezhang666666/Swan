using UnityEngine;
using Fungus;

/// <summary>
/// When the Player enters this trigger, automatically executes 
/// the specified Fungus block (if not already running).
/// </summary>
public class Interact_triggerFungus : MonoBehaviour
{
    [Header("Fungus Settings")]
    [SerializeField] private Flowchart flowchart;
    [SerializeField] private string blockName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // If we have a Flowchart & a valid block name, and 
            // no other Fungus block is currently running
            if (flowchart != null && !string.IsNullOrEmpty(blockName))
            {
                if (!flowchart.HasExecutingBlocks())
                {
                    Debug.Log($"Interact_triggerFungus: Executing block {blockName}.");
                    flowchart.ExecuteBlock(blockName);
                }
                else
                {
                    Debug.Log("Flowchart is already executing a block.");
                }
            }
            else
            {
                Debug.LogWarning("Flowchart or blockName not assigned in Interact_triggerFungus!");
            }
        }
    }
}
