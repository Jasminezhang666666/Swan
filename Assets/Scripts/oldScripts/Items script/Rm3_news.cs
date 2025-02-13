using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm3_news : Items
{
    [SerializeField] float delay = 3f;
    string emptyName = "Glass Empty";
    string fullName = "Glass Full";

    protected override void Start()
    {
        base.Start();
        interctDia.Add("The full cast of the upcoming ballet \"Swan Lake\" has been announced.");
        interctDia.Add("The protagonist of this dance, the Black Swan, will be played by XXX.");
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && InventoryStorage.instance.selectedItemId == emptyName)
        {
            if (InventoryStorage.instance.HasItem(emptyName))
            {
                InventoryStorage.instance.RemoveItem(emptyName);
            }

            StartCoroutine(Wait(delay));

            if(!InventoryStorage.instance.HasItem(fullName))
            {
                InventoryStorage.instance.AddItem(fullName);
            }
        }
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
