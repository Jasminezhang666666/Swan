using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm2_gear : Items
{
    protected override void Start()
    {
        canGoToInventory = true;

        base.Start();
        interctDia.Add("These plants have blossomed, but they're still talking");
        interctDia.Add("\"You slept with him for that role, didn't you? You never deserved to be the white swan!\".");
        interctDia.Add("There's a small gear on the flower.");
    }
}
