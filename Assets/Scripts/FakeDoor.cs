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

    private bool hasTriggered = false;

    // ← Make this overridable
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
            return;

        hasTriggered = true;

        // swap immediately
        if (objectToActivate != null) objectToActivate.SetActive(true);
        if (objectToDeactivate != null) objectToDeactivate.SetActive(false);

        // re‐bound camera
        if (cameraController != null)
            cameraController.SetBounds(newXBounds.x, newXBounds.y);

        // start fade
        if (fadeScript != null)
            fadeScript.FadeIn();
    }
}
