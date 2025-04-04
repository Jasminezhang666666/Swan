using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : EInteractable
{
    public GameObject NameTagUI; // Prefab for nameTag UI
    //private GameObject currentMapImage; // Reference to the current map image instance
    private bool isNameTagUIShowing = false; // To track if the nameTag UI is currently shown
    private Player player; // Reference to the Player script

    private void Start()
    {
        // Instantiate the map image prefab at the start but keep it inactive
        //currentMapImage = Instantiate(mapImagePrefab, Vector3.zero, Quaternion.identity);
        //currentMapImage.SetActive(false);

        // Get reference to Player component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.GetComponent<Player>() : FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        base.Interact();
        isNameTagUIShowing = !isNameTagUIShowing;
        NameTagUI.SetActive(isNameTagUIShowing);

        player.canMove = !isNameTagUIShowing;
    }
}

