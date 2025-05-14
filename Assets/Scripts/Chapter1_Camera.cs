using UnityEngine;
using System.Collections;

public class Chapter1_Camera : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player transform
    [SerializeField] private float smoothSpeed = 0.125f; // Smoothness of the camera movement
    [SerializeField] private Vector3 offset; // Offset for the camera relative to the player
    [SerializeField] private float xMinBound = -9f, xMaxBound = 9f; // X-axis boundaries

    // Optionally, assign a face transform to center when zooming in.
    [SerializeField] private Transform faceTransform;

    // Reference to the fade script (assign in Inspector)
    [SerializeField] private FadeScript fadeScript;

    private float fixedY; // Fixed Y position of the camera
    private float fixedZ; // Fixed Z position of the camera
    private float originalSize; // Store the original orthographic size

    // When true, the camera is locked in its zoomed state.
    private bool isZoomedIn = false;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found!");
        }

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        // Store the original orthographic size so we can restore it later.
        originalSize = cam.orthographicSize;

        // Find the player by tag if not assigned.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Please ensure the player has the 'Player' tag.");
            }
        }
    }

    private void LateUpdate()
    {
        // If zoomed in, do not update camera position.
        if (isZoomedIn)
            return;

        if (player != null)
        {
            // Calculate desired position based on the player's position and offset.
            float clampedX = Mathf.Clamp(player.position.x + offset.x, xMinBound, xMaxBound);
            Vector3 desiredPosition = new Vector3(clampedX, fixedY, fixedZ);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    /// <summary>
    /// Public method callable by Fungus to zoom in on the player's face.
    /// 'targetSize' is the final orthographic size, 'duration' is the zoom time.
    /// </summary>
    public void ZoomInOnFace(float targetSize, float duration, bool needExit)
    {
        // null-safe check: only flip the UI if instance and its inventory exist
        if (Inventory.instance != null
            && Inventory.instance.inventory != null
            && Inventory.instance.inventory.activeSelf)
        {
            Inventory.instance.TurnOnOffInventory();
        }
        StartCoroutine(ZoomInOnFaceCoroutine(targetSize, duration, needExit));
    }


    private IEnumerator ZoomInOnFaceCoroutine(float targetSize, float duration, bool needExit)
    {
        if (cam == null)
        {
            Debug.LogError("Camera component not found!");
            yield break;
        }
        if (faceTransform == null)
        {
            Debug.LogError("Face transform is not assigned!");
            yield break;
        }

        float startSize = cam.orthographicSize;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(faceTransform.position.x, faceTransform.position.y, fixedZ);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isZoomedIn = true;

        // Wait for 1 second after zooming in.
        yield return new WaitForSeconds(1f);

        if (needExit)
        {

            // Reset the camera once fade has finished
            ResetCamera();

            // Trigger a screen fade to black before resetting camera
            if (fadeScript != null)
            {
                fadeScript.FadeIn();
                // Wait for fade to complete (use the same duration as FadeScript)
                yield return new WaitForSeconds(fadeScript.fadeDuration);
            }

        }
    }

    /// <summary>
    /// Resets camera zoom and position to defaults.
    /// </summary>
    public void ResetCamera()
    {
        StopAllCoroutines();
        cam.orthographicSize = originalSize;
        if (player != null)
        {
            float clampedX = Mathf.Clamp(player.position.x + offset.x, xMinBound, xMaxBound);
            Vector3 defaultPosition = new Vector3(clampedX, fixedY, fixedZ);
            transform.position = defaultPosition;
        }
        isZoomedIn = false;
    }

    public void SetFollowTarget(Transform newTarget)
    {
        player = newTarget;
    }

    /// <summary>
    /// Change the horizontal follow bounds at runtime.
    /// </summary>
    public void SetBounds(float xMin, float xMax)
    {
        xMinBound = xMin;
        xMaxBound = xMax;
    }

}
