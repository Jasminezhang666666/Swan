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
    /// For an orthographic camera, zooming in is achieved by reducing its orthographic size.
    /// 'targetSize' is the final orthographic size (smaller means more zoom).
    /// 'duration' is the time over which the zoom occurs.
    /// </summary>
    public void ZoomInOnFace(float targetSize, float duration, bool needExit)
    {
        StartCoroutine(ZoomInOnFaceCoroutine(targetSize, duration, needExit));
    }

    /// <summary>
    /// Coroutine that smoothly interpolates the camera's orthographic size and position
    /// so that the faceTransform is centered. Once complete, the camera locks in the zoomed state.
    /// </summary>
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
        // Center the camera on the face, while keeping the fixed Z.
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

        /*
        // Ensure the final values are set.
        cam.orthographicSize = targetSize;
        transform.position = targetPos;
        */
        isZoomedIn = true;
        

        // Wait for 1 second after zooming in.
        yield return new WaitForSeconds(1f);

        // Then reset the camera to its original state.
        if (needExit)
        {
            ResetCamera();
        }
    }

    /// <summary>
    /// Public method callable by Fungus to reset the camera back to its original zoom and position.
    /// This stops any ongoing zoom coroutine and immediately resets the camera settings.
    /// </summary>
    public void ResetCamera()
    {
        // Stop any ongoing zoom coroutines
        StopAllCoroutines();
        // Reset the camera's orthographic size to the original value
        cam.orthographicSize = originalSize;
        // Recalculate the default position based on the player's position and offset
        if (player != null)
        {
            float clampedX = Mathf.Clamp(player.position.x + offset.x, xMinBound, xMaxBound);
            Vector3 defaultPosition = new Vector3(clampedX, fixedY, fixedZ);
            transform.position = defaultPosition;
        }
        // Resume normal camera follow behavior
        isZoomedIn = false;
    }

    public void SetFollowTarget(Transform newTarget)
    {
        player = newTarget;
    }


}
