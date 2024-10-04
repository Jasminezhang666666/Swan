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
            currentMapImage = Instantiate(mapImagePrefab, transform.position, Quaternion.identity);
            Debug.Log("Map image displayed.");
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
