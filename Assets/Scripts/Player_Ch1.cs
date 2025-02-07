using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Fungus;  // Ensure you have Fungus imported

public class Player_Ch1 : Player
{
    [Header("Camera Targets")]
    // Assign this transform in the Inspector for the dressing room shot.
    public Transform cameraTarget1;

    [Header("Camera Movement Settings")]
    public float cameraMoveDuration = 1.0f; // Duration for the camera to move between positions.

    // Optional: Reference to the regular camera movement script (Chapter1_Camera) so it can be disabled during the sequence.
    public Chapter1_Camera chapterCamera;

    // This will store the original camera position (retrieved automatically).
    private Vector3 originalCameraPosition;


    /// <summary>
    /// Awake is used instead of Start because the base Player.Start() is private.
    /// If we are in the dressing room scene, we store the camera's starting position and begin the camera sequence.
    /// </summary>
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Rm_DressingRoom01")
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Save the camera's original starting position (this becomes cameraTarget2).
                originalCameraPosition = mainCamera.transform.position;
            }
            else
            {
                Debug.LogError("Main Camera not found!");
            }

            // Start the dressing room camera sequence.
            StartCoroutine(DressingRoomCameraShow());
        }
    }

    /// <summary>
    /// Moves the camera to cameraTarget1, waits for 1.5 seconds, and then moves it back to its original position.
    /// During this sequence, player movement and the regular camera control are disabled.
    /// </summary>
    public IEnumerator DressingRoomCameraShow()
    {
        // Disable player movement.
        this.canMove = false;

        // Disable the regular camera movement if the Chapter1_Camera component is assigned.
        if (chapterCamera != null)
        {
            chapterCamera.enabled = false;
        }

        // Retrieve the main camera.
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }
        Transform camTransform = mainCamera.transform;

        // Smoothly move the camera to the first target position.
        yield return StartCoroutine(MoveCamera(camTransform, cameraTarget1.position, cameraMoveDuration));

        // Wait for 1.5 seconds at the first target.
        yield return new WaitForSeconds(1.5f);

        // Smoothly move the camera back to its original starting position.
        yield return StartCoroutine(MoveCamera(camTransform, originalCameraPosition, cameraMoveDuration));

        // Re-enable player movement.
        this.canMove = true;

        // Re-enable the regular camera movement.
        if (chapterCamera != null)
        {
            chapterCamera.enabled = true;
        }
    }

    /// <summary>
    /// Helper coroutine that smoothly moves the camera from its current position to a target position over a given duration.
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
        cam.position = target; // Ensure the camera lands exactly at the target position.
    }

    /// <summary>
    /// In the dressing room scene, if the player presses the left key, trigger Fungus block "4-2" (only once).
    /// This LateUpdate runs after the base class's Update so it does not interfere with the base input handling.
    /// </summary>
    private void LateUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Rm_DressingRoom01")
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
    }
}
