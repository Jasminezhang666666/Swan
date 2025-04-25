using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FakeDoor_E : FakeDoor
{
    [Header("E Indicator")]
    [Tooltip("Your 'Press E' UI prompt GameObject")]
    [SerializeField] private GameObject eIndicator;

    // internal state
    private bool isPlayerInRange = false;
    private Collider2D playerCollider = null;

    // 1) Show the prompt (but don’t do the door logic yet)
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerCollider = other;
            if (eIndicator != null)
                eIndicator.SetActive(true);
        }
    }

    // 2) Hide the prompt when they walk away
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerCollider = null;
            if (eIndicator != null)
                eIndicator.SetActive(false);
        }
    }

    // 3) When they press E, hide the prompt and trigger the door
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // hide prompt
            if (eIndicator != null)
                eIndicator.SetActive(false);

            // run the base door logic:
            //   • activate/deactivate objects
            //   • shift the player by playerXOffset
            //   • update camera bounds
            //   • start the fade
            base.OnTriggerEnter2D(playerCollider);

            // clear state so it only happens once
            isPlayerInRange = false;
            playerCollider = null;
        }
    }
}
