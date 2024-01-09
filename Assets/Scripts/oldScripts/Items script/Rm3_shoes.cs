using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Rm3_shoes : Items
{
    [SerializeField] float delay = 1f;
    string fullName = "Glass Full";

    [Header("Swap")]
    Animator anim; // animator plays pouring animation
    [SerializeField] GameObject blackShoes;

    bool black = false; // store whether it is black
    string shoesPref = "ShoesPref";

    protected override void Start()
    {
        base.Start();

        blackShoes.SetActive(false); // disable by default

        anim = GetComponent<Animator>(); // get animator
        anim.enabled = false;

        if (PlayerPrefs.HasKey(shoesPref)) // if clock state already exists
        {
            black = PlayerPrefs.GetInt(shoesPref) == 1 ? true : false; // load saved state
            SetBlack(black);
        }

        interctDia.Add("A brand new pair of ballet shoes. It must belong to a new dancer");
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && InventoryStorage.instance.selectedItemId == fullName)
        {
            anim.enabled = true; // start animation
            if (InventoryStorage.instance.HasItem(fullName))
            {
                InventoryStorage.instance.RemoveItem(fullName);
            }

            black = true;
            StartCoroutine(Wait(delay, black));
        }
    }

    IEnumerator Wait(float seconds, bool state)
    {
        yield return new WaitForSeconds(seconds);
        SetBlack(state);
    }

    void SetBlack(bool state)
    {
        black = state;
        blackShoes.SetActive(state);

        if (state) // if black
        {
            PlayerPrefs.SetInt(shoesPref, state ? 1 : 0);
            Inactivate();
        } else
        {
            PlayerPrefs.SetInt(shoesPref, state ? 1 : 0);
        }
    }

    void Inactivate()
    {
        //gameObject.GetComponent<SpriteRenderer>().enabled = false;
        //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //anim.enabled = false;
        Destroy(gameObject);
    }
}
