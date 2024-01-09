using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm1_cup : Items
{
    protected override void Start()
    {
        base.Start();
        canGoToInventory = true;

        interctDia.Add("A glass goblet.");
    }
}
