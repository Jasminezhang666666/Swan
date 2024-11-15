using UnityEngine;

public class Map : EInteractable
{
    public GameObject mapImagePrefab; // Prefab for the map image (Canvas)
    private GameObject currentMapImage; // Reference to the current map image instance
    private bool isMapShowing = false; // To track if the map is currently shown
    private Player player; // Reference to the Player script

    private void Start()
    {
        // Instantiate the map image prefab at the start but keep it inactive
        currentMapImage = Instantiate(mapImagePrefab, Vector3.zero, Quaternion.identity);
        currentMapImage.SetActive(false);

        // Get reference to Player component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.GetComponent<Player>() : FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        base.Interact();

        // Toggle the map display
        isMapShowing = !isMapShowing;

        // Toggle the display of the map image when interacting
        if (currentMapImage != null)
        {
            currentMapImage.SetActive(isMapShowing);
            Debug.Log(isMapShowing ? "Map image displayed." : "Map image hidden.");
        }

        // Enable or disable the player's movement based on map display status
        if (player != null)
        {
            player.canMove = !isMapShowing;
        }
    }
}
