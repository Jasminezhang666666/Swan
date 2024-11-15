using UnityEngine;
using System.Collections;
using Fungus;

public class Jane : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2f;      // Speed at which Jane moves.
    [SerializeField] private Transform[] locations;        // Array of locations Jane will move to.

    [Header("Dialogue Settings")]
    [SerializeField] private Flowchart dialogueFlowchart;    // Single Flowchart for all dialogues.
    [SerializeField] private string firstDialogueBlock;      // Block name for first dialogue.
    [SerializeField] private string secondDialogueBlock;     // Block name for second dialogue.
    [SerializeField] private float dialogueDelay = 3f;       // Time to wait during dialogue.

    [Header("Disappearance Settings")]
    [SerializeField] private float fadeOutDuration = 2f;     // Duration for fade-out effect.

    public AK.Wwise.Event onFootstep;
    private uint onFootstep_playingID;


    private bool isMoving = false;                         // Indicates if Jane is currently moving.
    private Vector3 originalScale;                         // Stores Jane's original scale for flipping.
    private bool isInDialogue = false;                     // Prevents multiple dialogues simultaneously.

    // Enum to manage Jane's states
    private enum JaneState
    {
        MovingToFirstLocation,
        WaitingAtFirstLocation,
        MovingToSecondLocation,
        WaitingAtSecondLocation,
        MovingToThirdLocation,
        Disappeared
    }

    private JaneState currentState;

    private void Start()
    {
        // Store Jane's original local scale.
        originalScale = transform.localScale;

        // Initialize state to start moving to the first location.
        currentState = JaneState.MovingToFirstLocation;
        isMoving = true;
    }

    private void Update()
    {
        // Check if the current chapter is Chapter1
        if (ChapterManager.Instance == null ||ChapterManager.Instance.CurrentChapter != ChapterManager.Chapter.Chapter1)
        {
            // If not Chapter1, do not execute Jane's behaviors
            return;
        }

        if (isMoving)
        {

            if (onFootstep_playingID == 0)
            {
                onFootstep_playingID = onFootstep.Post(this.gameObject);
            }



            switch (currentState)
            {
                case JaneState.MovingToFirstLocation:
                    MoveToLocation(locations[0].position);
                    break;
                case JaneState.MovingToSecondLocation:
                    MoveToLocation(locations[1].position);
                    break;
                case JaneState.MovingToThirdLocation:
                    MoveToLocation(locations[2].position);
                    break;
                default:
                    break;
            }
        } else
        {
            if (onFootstep_playingID != 0)
            {
                AkSoundEngine.StopPlayingID(onFootstep_playingID);
                onFootstep_playingID = 0;
            }


        }
    }

    /// <summary>
    /// Moves Jane towards the specified target position.
    /// </summary>
    /// <param name="targetPosition">The position to move towards.</param>
    private void MoveToLocation(Vector3 targetPosition)
    {
        // Move Jane towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);

        // Determine direction to face based on target position.
        if (targetPosition.x < transform.position.x)
        {
            // Face left if the target is to the left.
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (targetPosition.x > transform.position.x)
        {
            // Face right if the target is to the right.
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Check if Jane has reached the target position.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            HandleArrivalAtLocation();
        }
    }

    /// <summary>
    /// Handles actions upon arriving at a location based on the current state.
    /// </summary>
    private void HandleArrivalAtLocation()
    {
        switch (currentState)
        {
            case JaneState.MovingToFirstLocation:
                // After reaching first location, flip to face left.
                FlipDirection();
                currentState = JaneState.WaitingAtFirstLocation;
                break;

            case JaneState.MovingToSecondLocation:
                // After reaching second location, flip to face left.
                FlipDirection();
                currentState = JaneState.WaitingAtSecondLocation;
                break;

            case JaneState.MovingToThirdLocation:
                // After reaching third location, start disappearing.
                currentState = JaneState.Disappeared;
                StartCoroutine(Disappear());
                break;
        }
    }

    /// <summary>
    /// Flips Jane's sprite to face the opposite direction.
    /// </summary>
    private void FlipDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Detects collision with the player to trigger dialogue.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        // Ensure the collided object is the player and Jane isn't already in dialogue.
        if (collision.gameObject.CompareTag("Player") && !isInDialogue)
        {
            if (currentState == JaneState.WaitingAtFirstLocation)
            {
                TriggerDialogue(firstDialogueBlock);
            }
            else if (currentState == JaneState.WaitingAtSecondLocation)
            {
                TriggerDialogue(secondDialogueBlock);
            }
        }
    }

    /// <summary>
    /// Triggers the specified Fungus dialogue block.
    /// </summary>
    /// <param name="blockName">The name of the block to execute.</param>
    private void TriggerDialogue(string blockName)
    {
        if (dialogueFlowchart == null || string.IsNullOrEmpty(blockName))
        {
            Debug.LogWarning("Flowchart or BlockName not assigned in Jane script.");
            return;
        }

        if (dialogueFlowchart.HasExecutingBlocks())
        {
            Debug.Log("Flowchart is already executing a block.");
            return;
        }

        Debug.Log($"Triggering dialogue block: {blockName}");
        dialogueFlowchart.ExecuteBlock(blockName);
        isInDialogue = true;
        StartCoroutine(DialogueSequence());
    }

    /// <summary>
    /// Manages the dialogue sequence, including delays and movement to the next location.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator DialogueSequence()
    {
        // Wait for the dialogue to finish. Adjust this based on actual dialogue length or events.
        yield return new WaitForSeconds(dialogueDelay);
        isInDialogue = false;

        // Determine the next action based on the current state.
        if (currentState == JaneState.WaitingAtFirstLocation)
        {
            // Proceed to move to the second location.
            currentState = JaneState.MovingToSecondLocation;
            isMoving = true;
        }
        else if (currentState == JaneState.WaitingAtSecondLocation)
        {
            // Proceed to move to the third location.
            currentState = JaneState.MovingToThirdLocation;
            isMoving = true;
        }
    }

    /// <summary>
    /// Gradually fades out all SpriteRenderers in Jane and its children, then disables them.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator Disappear()
    {
        // Disable Animator to prevent it from overriding SpriteRenderer colors
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
        else
        {
            Debug.LogWarning("No Animator component found on Jane.");
        }

        // Retrieve all SpriteRenderers in Jane and its children
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found on Jane or its children.");
            yield break;
        }

        float elapsed = 0f;

        // Store original colors
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        // Gradually reduce alpha over time
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, alpha);
            }

            yield return null;
        }

        // Ensure all SpriteRenderers are fully transparent and disabled
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0f);
            spriteRenderers[i].enabled = false;
        }

        // Optionally, disable the entire GameObject if needed
        gameObject.SetActive(false);
    }
}
