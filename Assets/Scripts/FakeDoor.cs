using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class FakeDoor : MonoBehaviour
{
    [Header("Fade & Swap Settings")]
    [SerializeField] private FadeScript fadeScript;
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject objectToDeactivate;

    [Header("Camera Bounds")]
    [SerializeField] private Chapter1_Camera cameraController;
    [SerializeField] private Vector2 newXBounds = new Vector2(-9f, 9f);

    [Header("Player Position Offset")]
    [Tooltip("How far to shift the player's X when the door triggers")]
    [SerializeField] private float playerXOffset = 0f;

    private bool hasTriggered = false;

    // allow subclasses to override—but this is where we do the core logic
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
            return;

        hasTriggered = true;

        // 1) Activate the new object
        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        // 2) Shift the player
        Transform playerT = other.transform;
        Vector3 p = playerT.position;
        playerT.position = new Vector3(p.x + playerXOffset, p.y, p.z);

        // 3) Deactivate the old object
        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        // 4) Update camera bounds
        if (cameraController != null)
            cameraController.SetBounds(newXBounds.x, newXBounds.y);

        // 5) Play fade
        if (fadeScript != null)
            fadeScript.FadeIn();
    }
}
