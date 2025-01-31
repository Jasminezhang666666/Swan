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
    [SerializeField] private string firstDialogueBlock;   // e.g. "Jane_Hallway_1"
    [SerializeField] private string secondDialogueBlock;  // e.g. "Jane_Hallway_2"
    [SerializeField] private float dialogueDelay = 3f;

    [Header("Dialogue Settings - Backstage")]
    // Normal path: "2-2" -> "2-3"
    // Stage-looked-at path: "2-4" -> "2-5"
    [SerializeField] private string backstageFirstBlock = "2-2";
    [SerializeField] private string backstageSecondBlock = "2-3";
    [SerializeField] private string backstageThirdBlock = "2-4";
    [SerializeField] private string backstageFourthBlock = "2-5";

    [Header("Scene Object Activation")]
    // These two GameObjects will be activated when certain dialogues are triggered
    [SerializeField] private GameObject eStageObject;         // Activate after "2-2"
    [SerializeField] private GameObject doorToHallwayObject;  // Activate after "2-5"

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
    //   (for each dialogue path).
    private int backstageCollisionCount = 0;

    // Whether Jane has arrived & flipped in backstage
    private bool backstageHasArrived = false;

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
        // Only proceed in Chapter1
        if (ChapterManager.Instance == null ||
            ChapterManager.Instance.CurrentChapter != ChapterManager.Chapter.Chapter1)
        {
            return;
        }

        // If we aren't in hallway or backstage scene, do nothing.
        if (!isHallwayScene && !isBackstageScene)
            return;

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
    private void MoveToLocation(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            walkingSpeed * Time.deltaTime
        );

        // Flip direction if necessary
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                 originalScale.y,
                 originalScale.z
            );
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                 Mathf.Abs(originalScale.x),
                 originalScale.y,
                 originalScale.z
            );
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
    /// Backstage: Flip once & mark that we have arrived.
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
            // Jane flips upon arriving
            FlipDirection();
            // Mark that she arrived & flipped
            backstageHasArrived = true;
        }
    }

    /// <summary>
    /// Flip Jane horizontally.
    /// </summary>
    private void FlipDirection()
    {
        transform.localScale = new Vector3(
            -transform.localScale.x,
             transform.localScale.y,
             transform.localScale.z
        );
    }

    /// <summary>
    /// On player collision, we trigger dialogues (Hallway or Backstage).
    /// </summary>
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
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
    /// Backstage collision logic.
    /// 1) Must have arrived/flipped before triggering anything.
    /// 2) If the player looked at stage, we do "2-4" then "2-5".
    /// 3) Otherwise, do "2-2" then "2-3".
    /// </summary>
    private void HandleBackstageCollision()
    {
        // If Jane hasn't arrived/flipped yet, do nothing
        if (!backstageHasArrived)
        {
            Debug.Log("Jane backstage: not ready to talk yet.");
            return;
        }

        // If the player has looked at the stage, do 2-4 -> 2-5
        if (ChapterManager.Instance.Chp1_LookedAtStage)
        {
            backstageCollisionCount++;
            if (backstageCollisionCount == 1)
            {
                // First collision after looking at stage => "2-4"
                TriggerDialogue(backstageThirdBlock);
            }
            else if (backstageCollisionCount == 2)
            {
                // Second collision => "2-5"
                TriggerDialogue(backstageFourthBlock);
            }
            else
            {
                Debug.Log("Jane backstage: no more dialogues (stage path).");
            }
        }
        else
        {
            // Otherwise do normal "2-2"/"2-3"
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
                Debug.Log("Jane backstage: no more dialogues (normal path).");
            }
        }
    }

    /// <summary>
    /// Execute a Fungus block by name, then immediately activate certain objects if needed.
    /// </summary>
    private void TriggerDialogue(string blockName)
    {
        if (dialogueFlowchart == null || string.IsNullOrEmpty(blockName))
        {
            Debug.LogWarning("Flowchart or BlockName not assigned in Jane script.");
            return;
        }

        // Activate E_Stage if we are triggering "2-2"
        if (blockName == backstageFirstBlock) // i.e. "2-2"
        {
            if (eStageObject != null)
            {
                eStageObject.SetActive(true);
                Debug.Log("Activating E_Stage object.");
            }
        }

        // Activate DoorToHallway if we are triggering "2-5"
        if (blockName == backstageFourthBlock) // i.e. "2-5"
        {
            if (doorToHallwayObject != null)
            {
                doorToHallwayObject.SetActive(true);
                Debug.Log("Activating DoorToHallway object.");
            }
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
    /// Wait for the dialogueDelay, then (for hallway only) move on if needed.
    /// Backstage does nothing special after dialogues end, 
    /// so you can add logic if you want.
    /// </summary>
    private IEnumerator DialogueSequence()
    {
        yield return new WaitForSeconds(dialogueDelay);
        isInDialogue = false;

        // If hallway, proceed to next location
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
        // If backstage, do nothing extra by default
    }

    /// <summary>
    /// Fade out Jane and disable the GameObject.
    /// </summary>
    private IEnumerator Disappear()
    {
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
