using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Fungus;

public class Player_Ch1 : Player
{
    [Header("Camera Target")]
    // Assign the appropriate camera target in each scene via the Inspector.
    public Transform cameraTarget;

    [Header("Camera Movement Settings")]
    public float cameraMoveDuration = 1.0f; // Duration to move the camera.
    public float fallbackWaitDuration = 1.5f; // Wait time at the target (adjustable in the Inspector).

    // Optional: Reference to the regular camera movement script (Chapter1_Camera).
    public Chapter1_Camera chapterCamera;

    // Reference to Jane to check when she starts moving.
    public Jane jane;

    // NEW: Reference to Katlyn so that when Katlyn is moving, player can't move.
    public Katlyn katlyn;

    // This stores the original camera position.
    private Vector3 originalCameraPosition;

    public bool clothChangeNeeded = false;

    private void Awake()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Save the camera's original starting position.
            originalCameraPosition = mainCamera.transform.position;
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }

        string currentScene = SceneManager.GetActiveScene().name;
        if (cameraTarget != null)
        {
            if (currentScene == "Rm_DressingRoom01")
            {
                StartCoroutine(ShowCameraSequence(cameraTarget));
            }
            else if (currentScene == "Rm_DanceStudio01")
            {
                // For Rm_DanceStudio02, start the sequence that waits 1 sec then moves the camera.
                StartCoroutine(ShowCameraSequenceDanceStudio(cameraTarget));
            }
        }
    }

    /// <summary>
    /// Moves the camera to the target position, waits a fixed amount of time,
    /// then moves it back to the original position. (For Rm_DressingRoom01)
    /// </summary>
    private IEnumerator ShowCameraSequence(Transform target)
    {
        // Disable player movement and regular camera control.
        this.canMove = false;
        if (chapterCamera != null)
        {
            chapterCamera.enabled = false;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }
        Transform camTransform = mainCamera.transform;

        // Smoothly move the camera to the target position.
        yield return StartCoroutine(MoveCamera(camTransform, target.position, cameraMoveDuration));

        // Wait at the target.
        yield return new WaitForSeconds(fallbackWaitDuration);

        // Smoothly move the camera back to its original position.
        yield return StartCoroutine(MoveCamera(camTransform, originalCameraPosition, cameraMoveDuration));

        // Re-enable player movement and camera control.
        this.canMove = true;
        if (chapterCamera != null)
        {
            chapterCamera.enabled = true;
        }
    }

    /// <summary>
    /// For Rm_DanceStudio02: Waits 1 second, moves the camera to the target, 
    /// then waits (adjustable) until ReturnCameraToPlayer() is called.
    /// </summary>
    private IEnumerator ShowCameraSequenceDanceStudio(Transform target)
    {
        // Disable player movement and regular camera control.
        this.canMove = false;
        if (chapterCamera != null)
        {
            chapterCamera.enabled = false;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }
        Transform camTransform = mainCamera.transform;

        // Wait 1 second before moving the camera.
        yield return new WaitForSeconds(1f);

        // Move the camera to the target position.
        yield return StartCoroutine(MoveCamera(camTransform, target.position, cameraMoveDuration));

        // Wait at the target for the duration set in the inspector.
        yield return new WaitForSeconds(fallbackWaitDuration);

        // At this point, the camera remains at the target until ReturnCameraToPlayer() is called.
    }

    /// <summary>
    /// Public function that Fungus can call to return the camera back to the player's view.
    /// Moves the camera back to its original position and re-enables movement.
    /// </summary>
    public void ReturnCameraToPlayer()
    {
        StartCoroutine(ReturnCameraCoroutine());
    }

    private IEnumerator ReturnCameraCoroutine()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }
        Transform camTransform = mainCamera.transform;

        // Smoothly move the camera back to its original position.
        yield return StartCoroutine(MoveCamera(camTransform, originalCameraPosition, cameraMoveDuration));

        // Re-enable player movement and camera control.
        if (chapterCamera != null)
        {
            chapterCamera.enabled = true;
        }

        if (SceneManager.GetActiveScene().name != "Rm_DanceStudio01")
        {
            this.canMove = true;
        }
    }

    /// <summary>
    /// Helper coroutine that smoothly moves the camera from its current position to a target position.
    /// </summary>
    private IEnumerator MoveCamera(Transform cam, Vector3 target, float duration)
    {
        Vector3 startPos = cam.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.position = target;
    }

    /// <summary>
    /// Public method to be called by Fungus (or Jane's script) once block "5-2" is complete.
    /// This re-enables player movement and camera control.
    /// This function remains unchanged.
    /// </summary>
    public void EnablePlayerMovement()
    {
        // only re-enable once Katlyn has fully returned
        if (katlyn != null && katlyn.IsMoving)
            return;

        // turn the Player script back on
        this.enabled = true;
        canMove = true;

        if (chapterCamera != null)
            chapterCamera.enabled = true;
    }


    public void DisablePlayerMovement()
    {
        // stop all Player_Update/FixedUpdate logic in one go
        this.enabled = false;

        // also immediately cut velocity so the sprite doesn�t slide
        canMove = false;
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

    }

    /// <summary>
    /// LateUpdate handles scene-specific input:
    /// - In Rm_DressingRoom01, pressing A/Left triggers Fungus block "4-2".
    /// - In Rm_DanceStudio02, pressing D/Right triggers Fungus block "5-3".
    /// </summary>
    private void LateUpdate()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Rm_DressingRoom02")
        {
            // only after puzzle flagged:
            if (clothChangeNeeded &&
               (Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.D)))
            {
                if (flowchart != null)
                    flowchart.ExecuteBlock("NeedToChange");
                else
                    Debug.LogError("Player_Ch1: flowchart not assigned!");
            }
        }
        else if (currentScene == "Rm_DressingRoom01")
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (flowchart != null)
                    flowchart.ExecuteBlock("4-2");
                else
                    Debug.LogError("Player_Ch1: flowchart not assigned!");
            }
        }
    }

    public void SetClothChangeNeeded(bool needed, Flowchart chart)
    {
        clothChangeNeeded = needed;
        flowchart = chart;
    }

}
