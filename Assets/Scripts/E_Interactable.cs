using UnityEngine;
using Fungus;

public class EInteractable : MonoBehaviour
{
    public Flowchart flowchart; // Reference to the Fungus Flowchart
    public string blockName;    // The name of the specific block to trigger for this interactable

    private bool isPlayerInRange = false; // To track if the player is within range

    private void Update()
    {
        // Check if player is in range and presses the E key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Trigger the Fungus block by name if not already executing a block
            if (flowchart != null)
            {
                if (!flowchart.HasExecutingBlocks())
                {
                    Debug.Log("Executing block: " + blockName); // Debug to confirm the block is being called
                    flowchart.ExecuteBlock(blockName);
                }
                else
                {
                    Debug.Log("Flowchart is already executing a block");
                }
            }
            else
            {
                Debug.LogWarning("Flowchart not assigned!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
