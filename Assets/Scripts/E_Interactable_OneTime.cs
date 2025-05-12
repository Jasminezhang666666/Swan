using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Interactable_OneTime : EInteractable
{
    private BoxCollider2D interactionCollider;
    private bool hasInteracted = false;

    protected override void Awake()
    {
        base.Awake();
        // Auto‐find the trigger collider on this GameObject
        interactionCollider = GetComponent<BoxCollider2D>();
        if (interactionCollider == null)
            Debug.LogWarning($"{name}: no BoxCollider2D found on same GameObject!");
    }

    protected override void Update()
    {
        if (hasInteracted) return;
        base.Update();
    }

    public override void Interact()
    {
        if (hasInteracted) return;
        base.Interact();
        hasInteracted = true;

        // Disable the trigger so the icon no longer shows
        if (interactionCollider != null)
            interactionCollider.enabled = false;

        // Hide the icon immediately
        if (prefabToActivate != null)
            prefabToActivate.SetActive(false);
    }
}