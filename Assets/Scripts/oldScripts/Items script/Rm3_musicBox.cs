using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Rm3_musicBox : Items
{
    [Header("Settings")]
    [SerializeField] float delay = 1f;
    [SerializeField] string nextSceneName;
    [Header("Sprites")]
    [SerializeField] Sprite figurineOn;

    string figurine = "Figurine";
    protected override void Start()
    {
        base.Start();
        interctDia.Add("The bottom part of a music box, you wonder where the upper figure is.");
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && Input.GetKeyDown(KeyCode.Return) && InventoryStorage.instance.selectedItemId == figurine)
        {
            if (InventoryStorage.instance.HasItem(figurine))
            {
                InventoryStorage.instance.RemoveItem(figurine);

                rend.sprite = figurineOn;
                StartCoroutine(Wait(delay));
            }
        }
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<SceneTransition>().SwitchScene(nextSceneName);
    }
}
