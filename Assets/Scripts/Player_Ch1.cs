using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Fungus;

// Ensure you have Fungus imported
public class Player_Ch1 : Player
{
    [Header("Camera Target")]
    // Assign the appropriate camera target in each scene via the Inspector.
    public Transform cameraTarget;

    [Header("Camera Movement Settings")]
    public float cameraMoveDuration = 1.0f; // Duration to move the camera.
    public float fallbackWaitDuration = 1.5f; // Wait time at the target.

    // Optional: Reference to the regular camera movement script (Chapter1_Camera).
    public Chapter1_Camera chapterCamera;

    // Reference to Jane to check when she starts moving.
    public Jane jane;

    // NEW: Reference to Catlyn so that when Catlyn is moving, player can't move.
    public Catlyn catlyn;

    // This stores the original camera position.
    private Vector3 originalCameraPosition;

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

        // For Rm_DressingRoom01, run the camera sequence if a camera target is assigned.
        if (SceneManager.GetActiveScene().name == "Rm_DressingRoom01" && cameraTarget != null)
        {
            StartCoroutine(ShowCameraSequence(cameraTarget));
        }
    }

    /// <summary>
    /// Moves the camera to the target position, waits a short while, then moves it back.
    /// This disables player movement and regular camera control during the sequence.
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

        // In Rm_DressingRoom01, wait a fixed amount of time.
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
    /// </summary>
    public void EnablePlayerMovement()
    {
        // Wait until Catlyn is finished moving (if Catlyn is assigned).
        if (catlyn != null && !catlyn.IsMoving)
        {
            Debug.Log("Player movement enabled via EnablePlayerMovement()");
            this.canMove = true;
            if (chapterCamera != null)
            {
                chapterCamera.enabled = true;
            }
        }
    }

    /// <summary>
    /// LateUpdate handles scene-specific input:
    /// - In Rm_DressingRoom01, pressing A/Left triggers Fungus block "4-2".
    /// - In Rm_DanceStudio02, pressing D/Right triggers Fungus block "5-3".
    /// </summary>
    private void LateUpdate()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Rm_DressingRoom01")
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (flowchart != null)
                {
                    flowchart.ExecuteBlock("4-2");
                }
                else
                {
                    Debug.LogError("Flowchart not assigned!");
                }
            }
        }
        else if (currentScene == "Rm_DanceStudio02")
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (flowchart != null)
                {
                    flowchart.ExecuteBlock("5-3");
                }
                else
                {
                    Debug.LogError("Flowchart not assigned!");
                }
            }
        }
    }
}
