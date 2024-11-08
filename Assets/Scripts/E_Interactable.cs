using UnityEngine;
using Fungus;

public class EInteractable : MonoBehaviour
{
    public Flowchart flowchart; // Reference to the Fungus Flowchart
    public string blockName;    // The name of the specific block to trigger for this interactable
    [SerializeField] private GameObject prefabToActivate; // Reference to the prefab to activate/deactivate
    protected bool isPlayerInRange = false; // To track if the player is within range
    private Player player; // Reference to the Player script

    private void Awake()
    {
        // Try to find the player by tag first
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            // If finding by tag fails, use FindObjectOfType as a fallback
            player = FindObjectOfType<Player>();
        }
        else
        {
            player = playerObject.GetComponent<Player>();
        }
    }

    private void Update()
    {
        // Only allow interaction if the player is in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interact called for: " + gameObject.name);

        // Handle Fungus block execution
        if (!string.IsNullOrEmpty(blockName) && flowchart != null)
        {
            if (!flowchart.HasExecutingBlocks())
            {
                Debug.Log("Executing block: " + blockName);
                flowchart.ExecuteBlock(blockName);
            }
        }
        else
        {
            Debug.LogWarning("No blockName provided or Flowchart not assigned!");
        }

        // Toggle the prefab's active state if it exists
        if (prefabToActivate != null)
        {
            prefabToActivate.SetActive(!prefabToActivate.activeSelf);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Activate the prefab when the player is in range
            if (prefabToActivate != null)
            {
                prefabToActivate.SetActive(true);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the collider
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Deactivate the prefab when the player exits
            if (prefabToActivate != null)
            {
                prefabToActivate.SetActive(false);
            }
        }
    }
}
