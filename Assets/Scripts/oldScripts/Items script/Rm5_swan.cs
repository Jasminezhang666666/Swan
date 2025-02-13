using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm5_swan : Items
{
    [Header("Settings")]
    [SerializeField] AudioSource source;
    [SerializeField] float delay = 1f;
    Animator anim;

    string knife = "Knife";
    string figurine = "Figurine";

    [Header("Sprites")]
    [SerializeField] Sprite swanWithFigure;
    [SerializeField] Sprite swanNoFigure;

    enum SwanState
    { // OMG clown!! --Faustine, why were you using ints
        Normal,
        CutWithFigure,
        Cut
    }
    SwanState state = SwanState.Normal;
    string swanPref = "SwanPref";

    protected override void Start()
    {
        base.Start();
        interctDia.Add("Dead white swan. Reminds you of your past self.");

        anim = GetComponent<Animator>();
        anim.enabled = false;

        if (PlayerPrefs.HasKey(swanPref)) // if clock state already exists
        {
            state = (SwanState)PlayerPrefs.GetInt(swanPref); // load saved state
            UpdateState(state);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && state == SwanState.Normal && Input.GetKeyDown(KeyCode.Return) && InventoryStorage.instance.selectedItemId == knife)
        {
            if (InventoryStorage.instance.HasItem(knife))
            {
                InventoryStorage.instance.RemoveItem(knife);

                StartCoroutine(WaitForAnim(delay));
            }
        }
    }

    private void UpdateState(SwanState input)
    {
        state = input;
        PlayerPrefs.SetInt(swanPref, (int)state); // set state

        switch (state)
        {
            case SwanState.Normal:
                break;
            case SwanState.CutWithFigure:
                UpdateSprites(swanWithFigure, swanWithFigure);
                break;
            case SwanState.Cut:
                UpdateSprites(swanNoFigure, swanNoFigure);
                break;
            default:
                break;
        }
    }

    private void UpdateSprites(Sprite og, Sprite highlighted)
    {
        rend.sprite = og;
        originalPic = og;
        highlightedPic = highlighted;
    }

    protected override void Interact()
    {
        base.Interact();

        if (state == SwanState.CutWithFigure)
        {
            if (!InventoryStorage.instance.HasItem(figurine))
            {
                InventoryStorage.instance.AddItem(figurine);
            }

            state = SwanState.Cut;
            UpdateState(state);
        }
    }

    IEnumerator WaitForAnim(float time)
    {
        anim.enabled = true;
        yield return new WaitForSeconds(time);
        state = SwanState.CutWithFigure;
        UpdateState(state);
        source.Play();
        anim.enabled = false;
    }
}
