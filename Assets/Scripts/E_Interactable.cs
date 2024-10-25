using UnityEngine;
using Fungus;

public class EInteractable : MonoBehaviour
{
    public Flowchart flowchart; // Reference to the Fungus Flowchart
    public string blockName;    // The name of the specific block to trigger for this interactable
    [SerializeField] private GameObject prefabToActivate; // Reference to the prefab to activate/deactivate

    private bool isPlayerInRange = false; // To track if the player is within range

    private void Update()
    {
        // Only allow interaction if the player is in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // Method to handle interaction
    public virtual void Interact()
    {
        Debug.Log("Interact called for: " + gameObject.name);

        if (!string.IsNullOrEmpty(blockName) && flowchart != null)
        {
            if (!flowchart.HasExecutingBlocks())
            {
                Debug.Log("Executing block: " + blockName);
                flowchart.ExecuteBlock(blockName);
            }
            else
            {
                Debug.Log("Flowchart is already executing a block");
            }
        }
        else
        {
            Debug.LogWarning("No blockName provided or Flowchart not assigned!");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log($"{other.name} entered range of {gameObject.name}");

            // Activate the prefab when the player is in range
            if (prefabToActivate != null)
            {
                prefabToActivate.SetActive(true);
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log($"{other.name} exited range of {gameObject.name}");

            // Deactivate the prefab when the player exits
            if (prefabToActivate != null)
            {
                prefabToActivate.SetActive(false);
            }
        }
    }
}
