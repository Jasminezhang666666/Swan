// FakeDoor_E.cs
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

    // 1) Show the prompt when player enters
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

    // 2) Hide the prompt when player leaves
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

    // 3) Wait for E, then invoke base logic
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (eIndicator != null)
                eIndicator.SetActive(false);

            // run the full FakeDoor logic (activate, shift, deactivate, re‐bind, fade)
            base.OnTriggerEnter2D(playerCollider);

            // clear so you can re‐enter (or re‐press after exit/enter)
            isPlayerInRange = false;
            playerCollider = null;
        }
    }
}
