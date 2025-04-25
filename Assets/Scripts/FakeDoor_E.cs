using UnityEngine;

public class FakeDoor_E : FakeDoor
{
    [Header("E Indicator")]
    [Tooltip("Your 'Press E' UI prompt GameObject")]
    [SerializeField] private GameObject eIndicator;

    // track when the player is inside our trigger
    private bool isPlayerInRange = false;
    private Collider2D playerCollider;

    // 1) Show the prompt but DO NOT call base yet
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

    // 2) Hide the prompt when the player leaves
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

    // 3) Listen for E and then do the actual door logic once
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // hide the prompt
            if (eIndicator != null)
                eIndicator.SetActive(false);

            // now invoke the base door logic (swap, camera bounds, fade)
            base.OnTriggerEnter2D(playerCollider);

            // prevent double‐triggering
            isPlayerInRange = false;
            playerCollider = null;
        }
    }
}
