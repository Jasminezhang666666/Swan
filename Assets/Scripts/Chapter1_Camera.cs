using UnityEngine;

public class Chapter1_Camera : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player transform
    [SerializeField] private float smoothSpeed = 0.125f; // Smoothness of the camera movement
    [SerializeField] private Vector3 offset; // Offset position for the camera relative to the player
    [SerializeField] private float xMinBound = -9f, xMaxBound = 9f; // Camera X-axis boundaries

    private float fixedY; // Fixed Y axis position for the camera
    private float fixedZ; // Fixed Z axis position for the camera

    private void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        // Find player by tag if not assigned in the inspector
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
        if (player != null)
        {
            // Desired position with offset and boundaries for X-axis
            float clampedX = Mathf.Clamp(player.position.x + offset.x, xMinBound, xMaxBound);
            Vector3 desiredPosition = new Vector3(clampedX, fixedY, fixedZ);

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
