using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Rm1_key : Items
{
    protected override void Start()
    {
        base.Start();
        canGoToInventory = true;

        interctDia.Add("A door key.");
    }
}
