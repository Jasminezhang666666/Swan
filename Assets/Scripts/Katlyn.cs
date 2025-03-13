using UnityEngine;
using System.Collections;
using Fungus;

public class Katlyn : MonoBehaviour
{
    public bool IsMoving => isMoving;

    public enum CatlynState { Idle, MovingRight, MovingLeft }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform rightDestination; // Destination for moving right.
    [SerializeField] private Transform leftDestination;  // Destination for returning left.

    [Header("Fungus Settings")]
    [SerializeField] private Flowchart dialogueFlowchart;
    [SerializeField] private string fungusBlockName = "6-4"; // Fungus block triggered after right move.

    [Header("Camera Settings")]
    [SerializeField] private Chapter1_Camera cameraController; // Reference to the camera script.
    [SerializeField] private Transform playerTransform;        // Reference to the player transform.

    private CatlynState currentState = CatlynState.Idle;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Vector3 originalScale;

    // Cache a reference to the player's Player script.
    private Player playerRef;

    private void Start()
    {
        // Cache the player reference by tag.
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerRef = playerObj.GetComponent<Player>();
            if (playerRef == null)
            {
                Debug.LogWarning("Player component not found on the Player GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Make sure it is tagged 'Player'.");
        }

        // Ensure Katlyn starts facing left.
        originalScale = transform.localScale;
        if (originalScale.x > 0)
        {
            originalScale.x = -Mathf.Abs(originalScale.x);
            transform.localScale = originalScale;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            // Move Katlyn toward the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            Debug.Log("Katlyn moving. Current position: " + transform.position);

            // Check for arrival.
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                Debug.Log("Katlyn reached destination.");

                if (currentState == CatlynState.MovingRight)
                {
                    // Trigger the Fungus block after moving right.
                    if (dialogueFlowchart != null)
                    {
                        Debug.Log("Triggering Fungus block: " + fungusBlockName);
                        dialogueFlowchart.ExecuteBlock(fungusBlockName);
                    }
                    // Switch the camera to follow the player after the rightward move.
                    if (cameraController != null && playerTransform != null)
                    {
                        cameraController.SetFollowTarget(playerTransform);
                        Debug.Log("Camera now following the player.");
                    }
                }
                else if (currentState == CatlynState.MovingLeft)
                {
                    // Ensure the camera is following the player.
                    if (cameraController != null && playerTransform != null)
                    {
                        cameraController.SetFollowTarget(playerTransform);
                        Debug.Log("Camera now following the player.");
                    }
                    // Destroy Katlyn after reaching her second destination.
                    Debug.Log("Katlyn has been destroyed after reaching her second destination.");
                    Destroy(gameObject);
                }

                // Re-enable player movement now that Katlyn has stopped.
                if (playerRef != null)
                {
                    playerRef.canMove = true;
                }
                else
                {
                    Debug.LogWarning("Player reference is null; cannot re-enable movement.");
                }
                currentState = CatlynState.Idle;
            }
        }
    }

    /// <summary>
    /// Initiates Katlyn’s rightward movement.
    /// Katlyn flips to face right and moves to the assigned right destination.
    /// During this move, the camera follows Katlyn.
    /// When she stops, the Fungus block "6-4" is triggered and the camera follows the player.
    /// </summary>
    public void StartMoveRight()
    {
        // Disable player movement.
        if (playerRef != null)
        {
            playerRef.canMove = false;
        }
        else
        {
            Debug.LogWarning("Player reference not found; cannot disable movement.");
        }

        // Switch the camera to follow Katlyn during rightward movement.
        if (cameraController != null)
        {
            cameraController.SetFollowTarget(transform);
            Debug.Log("Camera now following Katlyn during right movement.");
        }
        else
        {
            Debug.LogWarning("Camera controller not assigned.");
        }

        // Flip Katlyn to face right.
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x);
        transform.localScale = newScale;
        Debug.Log("Katlyn flipped to face right.");

        currentState = CatlynState.MovingRight;
        if (rightDestination != null)
        {
            targetPosition = rightDestination.position;
        }
        else
        {
            Debug.LogWarning("Right destination not assigned.");
            return;
        }
        isMoving = true;
    }

    /// <summary>
    /// Initiates Katlyn’s leftward (return) movement.
    /// Immediately sets the camera to follow the player so it stops following Katlyn.
    /// Katlyn flips to face left, moves to the assigned left destination, and destroys herself upon arrival.
    /// </summary>
    public void StartMoveLeft()
    {
        // Disable player movement.
        if (playerRef != null)
        {
            playerRef.canMove = false;
        }
        else
        {
            Debug.LogWarning("Player reference not found; cannot disable movement.");
        }

        // Immediately set the camera to follow the player.
        if (cameraController != null && playerTransform != null)
        {
            cameraController.SetFollowTarget(playerTransform);
            Debug.Log("Camera set to follow the player as Katlyn starts moving left.");
        }

        // Flip Katlyn to face left.
        Vector3 newScale = transform.localScale;
        newScale.x = -Mathf.Abs(newScale.x);
        transform.localScale = newScale;
        Debug.Log("Katlyn flipped to face left.");

        currentState = CatlynState.MovingLeft;
        if (leftDestination != null)
        {
            targetPosition = leftDestination.position;
        }
        else
        {
            Debug.LogWarning("Left destination not assigned.");
            return;
        }
        isMoving = true;
    }
}
