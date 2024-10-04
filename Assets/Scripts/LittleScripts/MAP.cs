using UnityEngine;

public class Map : EInteractable
{
    public GameObject mapImagePrefab; // Prefab for the map image
    private GameObject currentMapImage; // Reference to the current map image instance

    public override void Interact()
    {
        // Toggle the display of the map image when interacting
        if (currentMapImage == null)
        {
            // Instantiate the map image prefab
            currentMapImage = Instantiate(mapImagePrefab, Vector3.zero, Quaternion.identity); // Set position to (0, 0, 0)
            Debug.Log("Map image displayed at (0, 0, 0).");
        }
        else
        {
            // Destroy the current map image if it exists
            Destroy(currentMapImage);
            currentMapImage = null;
            Debug.Log("Map image hidden.");
        }
    }
}
