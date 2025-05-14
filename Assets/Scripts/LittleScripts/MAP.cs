using UnityEngine;
using Fungus;

public class Map : EInteractable
{
    public GameObject mapImagePrefab; // Prefab for the map image (Canvas)
    private GameObject currentMapImage; // Reference to the instantiated map image
    private bool isMapShowing = false; // To track if the map is currently shown

    private void Start()
    {
        // Instantiate the map image prefab at the start but keep it inactive.
        if (mapImagePrefab != null)
        {
            currentMapImage = Instantiate(mapImagePrefab, Vector3.zero, Quaternion.identity);
            currentMapImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Map image prefab is not assigned!");
        }

        // Use the inherited 'player' field. If it hasn't been set in EInteractable, try to find it.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
            }
        }
    }

    // Override Update to add functionality for closing the map using Space
    protected override void Update()
    {
        // Call the base Update so that the E key still triggers an interaction via EInteractable
        base.Update();

        // If the player is in range, the map is currently displayed, and the player presses Space, toggle the map off.
        if (isPlayerInRange && isMapShowing && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

        //disable player movement
        player.canMove = !isMapShowing;
    }

    // Override Interact to toggle the map display and player movement
    public override void Interact()
    {
        // Call the base interaction to execute any Fungus blocks defined there.
        base.Interact();

        // Toggle the map display flag
        isMapShowing = !isMapShowing;

        // Toggle the map image active state accordingly
        if (currentMapImage != null)
        {
            currentMapImage.SetActive(isMapShowing);
            Debug.Log(isMapShowing ? "Map image displayed." : "Map image hidden.");
        }

        // Enable or disable the player's movement based on whether the map is visible
        if (player != null)
        {
            player.canMove = !isMapShowing;
        }
    }
}
