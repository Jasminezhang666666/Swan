// E_Hammer.cs
using UnityEngine;
using Fungus;

public class E_Hammer : EInteractable
{
    [Header("Fungus Prompts")]
    [SerializeField] private string needHammerBlockName;
    [SerializeField] private string needNailBlockName;

    private HammerAndNail hammerAndNail;

    protected override void Awake()
    {
        base.Awake();
        hammerAndNail = GetComponent<HammerAndNail>();
        if (hammerAndNail == null)
            Debug.LogError($"{name} needs a HammerAndNail component.");
    }

    public override void Interact()
    {
        if (!hammerAndNail.HammerEnabled)
        {
            if (!string.IsNullOrEmpty(needHammerBlockName))
                flowchart.ExecuteBlock(needHammerBlockName);
            return;
        }

        if (!hammerAndNail.NailEnabled)
        {
            if (!string.IsNullOrEmpty(needNailBlockName))
                flowchart.ExecuteBlock(needNailBlockName);
            return;
        }

        // both present → do the hammer logic
        hammerAndNail.StepHammer();
    }
}
