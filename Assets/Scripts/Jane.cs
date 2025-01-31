using UnityEngine;
using System.Collections;
using Fungus;
using UnityEngine.SceneManagement;

public class Jane : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2f;

    // Hallway: up to 3 locations
    [SerializeField] private Transform[] hallwayLocations;

    // Backstage: single location
    [SerializeField] private Transform backstageLocation;

    [Header("Dialogue Settings - Hallway")]
    [SerializeField] private Flowchart dialogueFlowchart;
    [SerializeField] private string firstDialogueBlock;      // e.g. "Jane_Hallway_1"
    [SerializeField] private string secondDialogueBlock;     // e.g. "Jane_Hallway_2"
    [SerializeField] private float dialogueDelay = 3f;

    [Header("Dialogue Settings - Backstage")]
    // We only need two blocks here: "2-2" and "2-3"
    [SerializeField] private string backstageFirstBlock = "2-2";
    [SerializeField] private string backstageSecondBlock = "2-3";

    [Header("Disappearance Settings")]
    [SerializeField] private float fadeOutDuration = 2f;

    public AK.Wwise.Event onFootstep;
    private uint onFootstep_playingID;

    private bool isMoving = false;
    private Vector3 originalScale;
    private bool isInDialogue = false;

    // Hallway states
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

    // Scene checks
    private bool isHallwayScene = false;
    private bool isBackstageScene = false;

    // Track how many times the Player has collided with Jane backstage
    private int backstageCollisionCount = 0;

    private void Start()
    {
        originalScale = transform.localScale;

        // Determine which scene we are in
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Rm_Hallway01")
        {
            isHallwayScene = true;
            currentState = JaneState.MovingToFirstLocation;
            isMoving = true;
        }
        else if (sceneName == "Rm_BackStage01")
        {
            isBackstageScene = true;
            isMoving = true;
        }
    }

    private void Update()
    {
        // Check Chapter 1 requirement
        if (ChapterManager.Instance == null || ChapterManager.Instance.CurrentChapter != ChapterManager.Chapter.Chapter1)
            return;

        // If we aren't in hallway or backstage scene, do nothing.
        if (!isHallwayScene && !isBackstageScene) return;

        if (isMoving)
        {
            // Ensure footstep SFX
            if (onFootstep_playingID == 0)
            {
                onFootstep_playingID = onFootstep.Post(this.gameObject);
            }

            if (isHallwayScene)
            {
                HandleHallwayMovement();
            }
            else if (isBackstageScene)
            {
                HandleBackstageMovement();
            }
        }
        else
        {
            // Stop footsteps if we are not moving
            if (onFootstep_playingID != 0)
            {
                AkSoundEngine.StopPlayingID(onFootstep_playingID);
                onFootstep_playingID = 0;
            }
        }
    }

    /// <summary>
    /// Manages Jane's movement in the hallway across 3 locations.
    /// </summary>
    private void HandleHallwayMovement()
    {
        switch (currentState)
        {
            case JaneState.MovingToFirstLocation:
                MoveToLocation(hallwayLocations[0].position);
                break;
            case JaneState.MovingToSecondLocation:
                MoveToLocation(hallwayLocations[1].position);
                break;
            case JaneState.MovingToThirdLocation:
                MoveToLocation(hallwayLocations[2].position);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Manages Jane's movement in the backstage (single location).
    /// </summary>
    private void HandleBackstageMovement()
    {
        if (backstageLocation == null)
        {
            Debug.LogWarning("backstageLocation is not assigned!");
            return;
        }

        MoveToLocation(backstageLocation.position);
    }

    /// <summary>
    /// Moves Jane towards the given target position at walkingSpeed.
    /// </summary>
    /// <param name="targetPosition"></param>
    private void MoveToLocation(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);

        // Flip direction if necessary
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Check arrival
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            HandleArrivalAtLocation();
        }
    }

    /// <summary>
    /// Called when Jane arrives at a target location. 
    /// Hallway: Manage states. 
    /// Backstage: Flip once.
    /// </summary>
    private void HandleArrivalAtLocation()
    {
        if (isHallwayScene)
        {
            switch (currentState)
            {
                case JaneState.MovingToFirstLocation:
                    FlipDirection();
                    currentState = JaneState.WaitingAtFirstLocation;
                    break;

                case JaneState.MovingToSecondLocation:
                    FlipDirection();
                    currentState = JaneState.WaitingAtSecondLocation;
                    break;

                case JaneState.MovingToThirdLocation:
                    currentState = JaneState.Disappeared;
                    StartCoroutine(Disappear());
                    break;
            }
        }
        else if (isBackstageScene)
        {
            // Jane flips immediately upon arriving
            FlipDirection();
            // No further movement. She'll just stand there.
        }
    }

    /// <summary>
    /// Flip Jane horizontally.
    /// </summary>
    private void FlipDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// On player collision, we trigger dialogues (Hallway or Backstage).
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        // Must collide with Player; must not already be in a dialogue
        if (!collision.gameObject.CompareTag("Player") || isInDialogue)
            return;

        if (isHallwayScene)
        {
            HandleHallwayCollision();
        }
        else if (isBackstageScene)
        {
            HandleBackstageCollision();
        }
    }

    /// <summary>
    /// Hallway collisions trigger up to two dialogues (firstBlock, secondBlock).
    /// </summary>
    private void HandleHallwayCollision()
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

    /// <summary>
    /// Backstage collisions trigger "2-2" the first time, "2-3" the second time.
    /// </summary>
    private void HandleBackstageCollision()
    {
        backstageCollisionCount++;

        if (backstageCollisionCount == 1)
        {
            TriggerDialogue(backstageFirstBlock);  // "2-2"
        }
        else if (backstageCollisionCount == 2)
        {
            TriggerDialogue(backstageSecondBlock); // "2-3"
        }
        else
        {
            Debug.Log("Jane backstage: no more dialogues set for further collisions.");
        }
    }

    /// <summary>
    /// Execute a Fungus block by name.
    /// </summary>
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
    /// Wait for the dialogueDelay, then (hallway) move on if needed.
    /// </summary>
    private IEnumerator DialogueSequence()
    {
        yield return new WaitForSeconds(dialogueDelay);
        isInDialogue = false;

        // If hallway, we proceed to next location after first or second block
        if (isHallwayScene)
        {
            if (currentState == JaneState.WaitingAtFirstLocation)
            {
                currentState = JaneState.MovingToSecondLocation;
                isMoving = true;
            }
            else if (currentState == JaneState.WaitingAtSecondLocation)
            {
                currentState = JaneState.MovingToThirdLocation;
                isMoving = true;
            }
        }
        // If backstage, do nothing special after the dialogue ends.
    }

    /// <summary>
    /// Fade out Jane and disable the GameObject.
    /// </summary>
    private IEnumerator Disappear()
    {
        // Stop any Animator from resetting sprite color
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found on Jane or its children.");
            yield break;
        }

        float elapsed = 0f;
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = new Color(
                    originalColors[i].r,
                    originalColors[i].g,
                    originalColors[i].b,
                    alpha
                );
            }
            yield return null;
        }

        // Final transparent & disable
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new Color(
                originalColors[i].r,
                originalColors[i].g,
                originalColors[i].b,
                0f
            );
            spriteRenderers[i].enabled = false;
        }

        gameObject.SetActive(false);
    }
}
