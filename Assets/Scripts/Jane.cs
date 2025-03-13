using UnityEngine;
using System.Collections;
using Fungus;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class Jane : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2f;
    // Hallway: up to 3 locations
    [SerializeField] private Transform[] hallwayLocations;
    // Backstage: single location
    [SerializeField] private Transform backstageLocation;
    // Dance Studio: single location for Rm_DanceStudio2
    [SerializeField] private Transform danceStudioLocation;

    [Header("Dialogue Settings - Hallway")]
    [SerializeField] private Flowchart dialogueFlowchart;
    [SerializeField] private string firstDialogueBlock;   // e.g. "Jane_Hallway_1"
    [SerializeField] private string secondDialogueBlock;  // e.g. "Jane_Hallway_2"
    [SerializeField] private float dialogueDelay = 3f;

    [Header("Dialogue Settings - Backstage")]
    // For Rm_BackStage01: "2-2" -> "2-3"
    // For Rm_BackStage02: "2-4" -> "2-5"
    [SerializeField] private string backstageFirstBlock = "2-2";
    [SerializeField] private string backstageSecondBlock = "2-3";
    [SerializeField] private string backstageThirdBlock = "2-4";
    [SerializeField] private string backstageFourthBlock = "2-5";

    [Header("Scene Object Activation")]
    // These GameObjects will be activated when certain dialogues are triggered.
    [SerializeField] private GameObject eStageObject;

    [Header("Disappearance Settings")]
    [SerializeField] private float fadeOutDuration = 2f;

    private bool stickAnimationPlaying = false;
    public bool IsReadyForMovement
    {
        get { return isMoving && !stickAnimationPlaying; }
    }

    public AK.Wwise.Event onFootstep;
    private uint onFootstep_playingID;

    // Animation component for switching between idle and walking animations.
    private Animator _animator;

    // References for idle sprite and bone rigging (walking) image.
    private SpriteRenderer idleSpr;
    private GameObject boneRiggingObj; // Assumed to be the first child

    public bool IsMoving { get { return isMoving; } }
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
    private bool isDanceStudioScene = false;

    // For DanceStudio movement control
    private bool isDanceStudioMovementStarted = false;
    private bool danceStudioDialogueTriggered = false;

    // Track backstage collision count for dialogue triggering.
    private int backstageCollisionCount = 0;
    // Whether Jane has arrived & flipped in backstage.
    private bool backstageHasArrived = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When returning to a backstage scene, reset dialogue counters and mark Jane as arrived.
        if (scene.name == "Rm_BackStage01" || scene.name == "Rm_BackStage02")
        {
            backstageCollisionCount = 0;
            backstageHasArrived = true; // Force ready for dialogue
            isBackstageScene = true;    // Ensure the backstage flag is set
            Debug.Log("Backstage scene reloaded: resetting dialogue counters and marking arrival.");
        }
    }

    private void Start()
    {
        originalScale = transform.localScale;

        // Initialize animator for animation switching.
        _animator = GetComponent<Animator>();

        // Get the idle sprite from the current GameObject.
        idleSpr = GetComponent<SpriteRenderer>();

        // Assume the first child is the bone rigging image for walking.
        if (transform.childCount > 0)
        {
            boneRiggingObj = transform.GetChild(0).gameObject;
        }

        // Set initial visual states.
        if (idleSpr != null)
            idleSpr.enabled = true;
        if (boneRiggingObj != null)
            boneRiggingObj.SetActive(false);

        // Determine which scene we are in.
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Rm_Hallway01")
        {
            isHallwayScene = true;
            currentState = JaneState.MovingToFirstLocation;
            isMoving = true;
        }
        else if (sceneName == "Rm_BackStage01" || sceneName == "Rm_BackStage02")
        {
            isBackstageScene = true;
            if (sceneName == "Rm_BackStage02")
            {
                // For Rm_BackStage02, Jane should not move, and she should face left.
                isMoving = false;
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else
            {
                isMoving = true;
            }
        }
        else if (sceneName == "Rm_DanceStudio02")
        {
            isDanceStudioScene = true;
            isMoving = false;
            isDanceStudioMovementStarted = false;
        }
    }

    private void Update()
    {
        // Only proceed in Chapter1.
        if (ChapterManager.Instance == null ||
            ChapterManager.Instance.CurrentChapter != ChapterManager.Chapter.Chapter1)
        {
            if (_animator != null)
                _animator.SetBool("isMoving", false);
            // Ensure visual representation is set to idle when not in Chapter1.
            if (idleSpr != null)
                idleSpr.enabled = true;
            if (boneRiggingObj != null)
                boneRiggingObj.SetActive(false);
            return;
        }

        // If we aren't in any of the handled scenes, do nothing.
        if (!isHallwayScene && !isBackstageScene && !isDanceStudioScene)
            return;

        if (isMoving)
        {
            // Ensure footstep SFX.
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
            else if (isDanceStudioScene)
            {
                HandleDanceStudioMovement();
            }
        }
        else
        {
            // Stop footsteps if not moving.
            if (onFootstep_playingID != 0)
            {
                AkSoundEngine.StopPlayingID(onFootstep_playingID);
                onFootstep_playingID = 0;
            }
        }

        // Update the animator parameter.
        if (_animator != null)
        {
            _animator.SetBool("isMoving", isMoving);
        }

        // When moving, disable the idle sprite and enable bone rigging.
        // When idle, enable the idle sprite and disable bone rigging.
        if (idleSpr != null && boneRiggingObj != null)
        {
            if (isMoving)
            {
                idleSpr.enabled = false;
                boneRiggingObj.SetActive(true);
            }
            else
            {
                idleSpr.enabled = true;
                boneRiggingObj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Handles movement in the hallway across multiple locations.
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
    /// Handles movement in the backstage scene.
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

    public void PlayStickAnimation()
    {
        if (SceneManager.GetActiveScene().name != "Rm_DanceStudio02")
        {
            Debug.LogWarning("PlayStickAnimation called in a non-dance studio scene.");
            Debug.Log("Active scene: " + SceneManager.GetActiveScene().name);
            return;
        }

        if (_animator != null)
        {
            Debug.Log("Playing stick animation: Jane_Stick");
            stickAnimationPlaying = true;
            _animator.Play("Jane_Stick", 0, 0f);
            StartCoroutine(WaitForStickAnimation());
        }
        else
        {
            Debug.LogWarning("Animator component not found on Jane.");
        }
    }

    private IEnumerator WaitForStickAnimation()
    {
        // Yield one frame to allow the state to update.
        yield return null;

        // Wait until the current animation state is no longer "Jane_Stick"
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jane_Stick"))
        {
            yield return null;
        }

        // Stick animation finished; allow movement.
        stickAnimationPlaying = false;
        isMoving = true;
    }

    private void HandleDanceStudioMovement()
    {
        if (stickAnimationPlaying)
        {
            // Stick animation is still playing; do not move.
            return;
        }

        if (danceStudioLocation == null)
        {
            Debug.LogWarning("danceStudioLocation is not assigned!");
            return;
        }
        MoveToLocation(danceStudioLocation.position);
    }

    /// <summary>
    /// Moves Jane toward the specified target position.
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

        // Check arrival at target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            HandleArrivalAtLocation();
        }
    }

    /// <summary>
    /// Called when Jane arrives at her target location.
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
            // In backstage, flip and mark arrival.
            FlipDirection();
            backstageHasArrived = true;
        }
        else if (isDanceStudioScene)
        {
            // In dance studio, trigger Fungus block "5-2" once Jane reaches the target.
            if (!danceStudioDialogueTriggered)
            {
                danceStudioDialogueTriggered = true;
                TriggerDialogue("5-2");
            }
        }
    }

    /// <summary>
    /// Flips Jane's sprite horizontally.
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
    /// When the player collides with Jane, trigger dialogues for hallway or backstage.
    /// </summary>
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        // Only proceed if the colliding object is the player,
        // if Jane isn't already in dialogue, and if she's not moving.
        if (!collision.gameObject.CompareTag("Player") || isInDialogue || isMoving)
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
    /// Handles hallway collisions to trigger dialogues.
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
    /// Handles backstage collisions to trigger dialogues.
    /// </summary>
    private void HandleBackstageCollision()
    {
        if (!backstageHasArrived)
        {
            Debug.Log("Jane backstage: not ready to talk yet.");
            return;
        }

        backstageCollisionCount++;
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Rm_BackStage01")
        {
            if (backstageCollisionCount == 1)
            {
                TriggerDialogue(backstageFirstBlock); // "2-2"
            }
            else
            {
                TriggerDialogue(backstageSecondBlock); // "2-3"
            }
        }
        else if (sceneName == "Rm_BackStage02")
        {
            if (backstageCollisionCount == 1)
            {
                TriggerDialogue(backstageThirdBlock); // "2-4"
            }
            else
            {
                TriggerDialogue(backstageFourthBlock); // "2-5"
            }
        }
    }

    /// <summary>
    /// Triggers a Fungus dialogue block and manages any related scene object activations.
    /// </summary>
    private void TriggerDialogue(string blockName)
    {
        if (dialogueFlowchart == null || string.IsNullOrEmpty(blockName))
        {
            Debug.LogWarning("Flowchart or BlockName not assigned in Jane script.");
            return;
        }

        // Activate eStageObject if triggering "2-2".
        if (blockName == backstageFirstBlock)
        {
            if (eStageObject != null)
            {
                eStageObject.SetActive(true);
                Debug.Log("Activating E_Stage object.");
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
    /// Waits for dialogueDelay seconds, then (for hallway) advances Jane's state.
    /// Backstage and dance studio do not have extra logic after dialogues by default.
    /// </summary>
    private IEnumerator DialogueSequence()
    {
        yield return new WaitForSeconds(dialogueDelay);
        isInDialogue = false;

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
    }

    /// <summary>
    /// Fades out Jane and then disables her GameObject.
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

    /// <summary>
    /// Public function to be called by Fungus in Rm_DanceStudio2.
    /// This function starts Jane's movement toward the assigned dance studio location.
    /// </summary>
    public void StartDanceStudioMovement()
    {
        Debug.Log("StartDanceStudioMovement called");
        if (!isDanceStudioScene)
        {
            Debug.LogWarning("StartDanceStudioMovement called in a non-dance studio scene.");
            return;
        }
        isDanceStudioMovementStarted = true;
        isMoving = true;
    }
}
