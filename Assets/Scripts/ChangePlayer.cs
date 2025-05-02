using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    [Tooltip("The currently active player")]
    public GameObject player1;

    [Tooltip("The player to switch into")]
    public GameObject player2;

    [Tooltip("Your camera controller that should follow the new player")]
    public Chapter1_Camera cameraController;

    public void SwitchPlayer()
    {
        if (player1 == null || player2 == null)
        {
            Debug.LogError("ChangePlayer: both player1 and player2 must be assigned.");
            return;
        }

        // Position: copy only X from player1
        Vector3 p2pos = player2.transform.position;
        p2pos.x = player1.transform.position.x;
        player2.transform.position = p2pos;

        // Facing: flip direction match
        Vector3 p1Scale = player1.transform.localScale;
        Vector3 p2Scale = player2.transform.localScale;
        float mag = Mathf.Abs(p2Scale.x);
        p2Scale.x = (p1Scale.x < 0f) ? -mag : mag;
        player2.transform.localScale = p2Scale;

        // Swap them
        player1.SetActive(false);
        player2.SetActive(true);

        // camera follow player2 now
        if (cameraController != null)
        {
            cameraController.SetFollowTarget(player2.transform);
        }
        else
        {
            Debug.LogWarning("ChangePlayer: cameraController not set. Cannot retarget camera.");
        }
    }
}
