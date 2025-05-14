using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    [Tooltip("The currently active player (to be deactivated)")]
    public GameObject player1;

    [Tooltip("The player to switch into (to be activated)")]
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

        // 1) Match X position
        Vector3 p2pos = player2.transform.position;
        p2pos.x = player1.transform.position.x;
        player2.transform.position = p2pos;

        // 2) Mirror facing
        Vector3 p1Scale = player1.transform.localScale;
        Vector3 p2Scale = player2.transform.localScale;
        float mag = Mathf.Abs(p2Scale.x);
        p2Scale.x = (p1Scale.x < 0f) ? -mag : mag;
        player2.transform.localScale = p2Scale;

        // 3) Swap the GameObjects
        player1.SetActive(false);
        player2.SetActive(true);

        // 4) On the newly active player2, clear the cloth flag & re-enable movement:
        var newController = player2.GetComponent<Player_Ch1>();
        if (newController != null)
        {
            newController.SetClothChangeNeeded(false, null);
            newController.EnablePlayerMovement();
        }
        else
        {
            Debug.LogError("ChangePlayer: no Player_Ch1 found on player2!");
        }

        // 5) Re-target camera
        if (cameraController != null)
            cameraController.SetFollowTarget(player2.transform);
        else
            Debug.LogWarning("ChangePlayer: cameraController not set. Cannot retarget camera.");
    }
}
