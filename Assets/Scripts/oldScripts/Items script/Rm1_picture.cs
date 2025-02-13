using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm1_picture : Items
{
    protected override void Start()
    {
        canGoToInventory = true;
        // description = "Item: Gear";

        base.Start();
        interctDia.Add("Half of the picture, Is that You?");
        interctDia.Add("Who is standing next to you?");
    }
}
