using UnityEngine;
using Fungus;

public class Katlyn : MonoBehaviour
{
    public bool IsMoving => isMoving;

    public enum KatlynState { Idle, MovingRight, MovingLeft }

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

    [Header("Animation Settings")]
    // The rigging parts (child objects) used for the walk (bone) animation.
    [SerializeField] private GameObject riggingParts;

    private KatlynState currentState = KatlynState.Idle;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Vector3 originalScale;

    // Reference to the SpriteRenderer that controls the idle animation.
    private SpriteRenderer spriteRenderer;

    // Reference to the Animator component controlling the animations.
    private Animator _animator;

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

        // Automatically get the SpriteRenderer on the same GameObject (for idle animation).
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on Katlyn.");
        }
        else
        {
            // Start with the idle sprite enabled.
            spriteRenderer.enabled = true;
        }

        // Get the Animator component.
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning("Animator not found on Katlyn.");
        }

        // Ensure the rigging parts (walking animation) are initially disabled.
        if (riggingParts != null)
        {
            riggingParts.SetActive(false);
        }
    }

    private void Update()
    {
        // Update animator parameter "isMoving" so the Animator is aware of the current movement state.
        if (_animator != null)
        {
            _animator.SetBool("isMoving", isMoving);
        }

        if (isMoving)
        {
            // Move Katlyn toward the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if Katlyn has reached the destination.
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;

                if (currentState == KatlynState.MovingRight)
                {
                    // Trigger the Fungus block after moving right.
                    if (dialogueFlowchart != null)
                    {
                        dialogueFlowchart.ExecuteBlock(fungusBlockName);
                    }
                    // Set the camera to follow the player.
                    if (cameraController != null && playerTransform != null)
                    {
                        cameraController.SetFollowTarget(playerTransform);
                    }
                }
                else if (currentState == KatlynState.MovingLeft)
                {
                    // Trigger the Fungus block "6-5" before destroying Katlyn.
                    if (dialogueFlowchart != null)
                    {
                        dialogueFlowchart.ExecuteBlock("6-5");
                    }
                    // Set the camera to follow the player.
                    if (cameraController != null && playerTransform != null)
                    {
                        cameraController.SetFollowTarget(playerTransform);
                    }
                    // Destroy Katlyn after reaching her destination.
                    Destroy(gameObject);
                }

                // Re-enable player movement when Katlyn stops.
                if (playerRef != null)
                {
                    playerRef.canMove = true;
                }
                currentState = KatlynState.Idle;
            }
        }

        // Update the active animation based on the movement state.
        UpdateAnimationState();
    }

    /// <summary>
    /// Toggles between the idle sprite animation and the rigging (bone) walk animation.
    /// When moving, disables the SpriteRenderer and enables the rigging parts.
    /// When idle, does the reverse.
    /// </summary>
    private void UpdateAnimationState()
    {
        if (isMoving)
        {
            // Enable rigging parts (walking) and disable the SpriteRenderer.
            if (riggingParts != null && !riggingParts.activeSelf)
                riggingParts.SetActive(true);
            if (spriteRenderer != null && spriteRenderer.enabled)
                spriteRenderer.enabled = false;
        }
        else
        {
            // Enable the SpriteRenderer (idle) and disable rigging parts.
            if (spriteRenderer != null && !spriteRenderer.enabled)
                spriteRenderer.enabled = true;
            if (riggingParts != null && riggingParts.activeSelf)
                riggingParts.SetActive(false);
        }
    }

    /// <summary>
    /// Initiates Katlyn’s rightward movement.
    /// Disables player movement, flips Katlyn to face right, and sets the target position.
    /// </summary>
    public void StartMoveRight()
    {
        if (playerRef != null)
        {
            playerRef.canMove = false;
        }
        if (cameraController != null)
        {
            cameraController.SetFollowTarget(transform);
        }
        // Flip Katlyn to face right.
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x);
        transform.localScale = newScale;

        currentState = KatlynState.MovingRight;
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
        print("Is moving is true!!");
    }

    /// <summary>
    /// Initiates Katlyn’s leftward movement.
    /// Disables player movement, flips Katlyn to face left, and sets the target position.
    /// </summary>
    public void StartMoveLeft()
    {
        if (playerRef != null)
        {
            playerRef.canMove = false;
        }
        if (cameraController != null && playerTransform != null)
        {
            cameraController.SetFollowTarget(playerTransform);
        }
        // Flip Katlyn to face left.
        Vector3 newScale = transform.localScale;
        newScale.x = -Mathf.Abs(newScale.x);
        transform.localScale = newScale;

        currentState = KatlynState.MovingLeft;
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
        print("is moving is true!!!");
    }
}
