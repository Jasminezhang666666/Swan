using UnityEngine;
using Fungus;
using System.Collections.Generic;

public class E_Door_MultipleOutput : E_Door
{
    //THIS IS ONLY FOR Rm_DressingRoom02 for now


    [Header("Requirements")]
    [Tooltip("Has the player changed clothes?")]
    public bool ChangedCloth = false;
    [Tooltip("Has the player fixed the floor?")]
    public bool FixedFloor = false;

    [Header("Fungus Blocks")]
    [Tooltip("0: prompt to change clothes\n1: prompt to fix the floor")]
    public List<string> fungusBlocks = new List<string>();

    public override void Interact()
    {
        // only react if player is in range
        if (!isPlayerInRange)
            return;

        // neither task done: ask to change clothes
        if (!ChangedCloth && !FixedFloor)
        {
            if (flowchart != null && fungusBlocks.Count > 0)
                flowchart.ExecuteBlock(fungusBlocks[0]);
            else
                Debug.LogWarning("E_Door_MultipleOutput: missing flowchart or fungusBlocks[0]");
        }
        // clothes done but floor not fixed: ask to fix floor
        else if (ChangedCloth && !FixedFloor)
        {
            if (flowchart != null && fungusBlocks.Count > 1)
                flowchart.ExecuteBlock(fungusBlocks[1]);
            else
                Debug.LogWarning("E_Door_MultipleOutput: missing flowchart or fungusBlocks[1]");
        }
        // both tasks complete: proceed to open/load scene
        else
        {
            base.Interact();
        }
    }
}
