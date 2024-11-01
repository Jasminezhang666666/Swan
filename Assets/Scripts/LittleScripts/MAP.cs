using UnityEngine;

public class Map : EInteractable
{
    public GameObject mapImagePrefab; // Prefab for the map image (Canvas)
    private GameObject currentMapImage; // Reference to the current map image instance

    private void Start()
    {
        // Instantiate the map image prefab at the start but keep it inactive
        currentMapImage = Instantiate(mapImagePrefab, Vector3.zero, Quaternion.identity);
        currentMapImage.SetActive(false);
    }

    public override void Interact()
    {
        // Toggle the display of the map image when interacting
        if (currentMapImage != null)
        {
            bool isActive = currentMapImage.activeSelf;
            currentMapImage.SetActive(!isActive);
            Debug.Log(isActive ? "Map image hidden." : "Map image displayed.");
        }
    }
}
