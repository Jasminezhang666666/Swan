using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm4_knife : Items
{
    protected override void Start()
    {
        canGoToInventory = true;

        base.Start();
        interctDia.Add("A sharp knife.");
    }
}
