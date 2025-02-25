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
    public void ZoomInOnFace(float targetSize, float duration)
    {
        StartCoroutine(ZoomInOnFaceCoroutine(targetSize, duration));
    }

    /// <summary>
    /// Coroutine that smoothly interpolates the camera's orthographic size and position
    /// so that the faceTransform is centered. Once complete, the camera locks in the zoomed state.
    /// </summary>
    private IEnumerator ZoomInOnFaceCoroutine(float targetSize, float duration)
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
        // The target position is centered on the faceTransform (keeping the fixed Z).
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
        cam.orthographicSize = targetSize;
        transform.position = targetPos;
        isZoomedIn = true;
    }

    public void SetFollowTarget(Transform newTarget)
    {
        player = newTarget;
    }

}
