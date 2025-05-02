using UnityEngine;

public class E_Hammer : EInteractable
{
    private HammerAndNail hammerAndNail;

    protected override void Awake()
    {
        base.Awake();
        hammerAndNail = GetComponent<HammerAndNail>();
        if (hammerAndNail == null)
            Debug.LogError($"E_Hammer on {name} requires a HammerAndNail component.");
    }

    public override void Interact()
    {
        // only step the hammer when in range
        if (isPlayerInRange && hammerAndNail != null)
        {
            hammerAndNail.StepHammer();
        }
    }
}
