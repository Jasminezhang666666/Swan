using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class R3_pic : Items
{
    [Header("Settings")]
    [SerializeField] AudioSource source;
    string picLeft = "Picture Left";
    string candleName = "Candle Unlit";

    [Header("Sprites")]
    [SerializeField] Sprite fullpicWithCandle;
    [SerializeField] Sprite fullpicCandleHighlight;
    [SerializeField] Sprite fullpicNoCandle;

    enum PicState { // OMG clown!! --Faustine, why were you using ints
        Half,
        WithCandle,
        NoCandle
    }
    PicState state = PicState.Half;
    string picPref = "PicPref";

    protected override void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey(picPref)) // if clock state already exists
        {
            state = (PicState)PlayerPrefs.GetInt(picPref); // load saved state
            UpdateState(state);
        }

        //if (state == PicState.Half)
        //{
        interctDia.Add("A ripped photo.");
        interctDia.Add("Depicts a well dressed man with curly brown hair.");
        //} else
        //{
        //    interctDia.Add("A ripped photo.");
        //}
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && state == PicState.Half && InventoryStorage.instance.selectedItemId == picLeft)
        {
            if (InventoryStorage.instance.HasItem(picLeft))
            {
                InventoryStorage.instance.RemoveItem(picLeft);
            }

            state = PicState.WithCandle;
            UpdateState(state);
            source.Play();
        }
    }

    private void UpdateState(PicState input)
    {
        state = input;
        PlayerPrefs.SetInt(picPref, (int)state); // set state

        switch (state)
        {
            case PicState.Half:
                break;
            case PicState.WithCandle:
                UpdateSprites(fullpicWithCandle, fullpicCandleHighlight);
                break;
            case PicState.NoCandle:
                UpdateSprites(fullpicNoCandle, fullpicNoCandle);
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

        if(state == PicState.WithCandle)
        {
            if (!InventoryStorage.instance.HasItem(candleName))
            {
                InventoryStorage.instance.AddItem(candleName);
            }

            state = PicState.NoCandle;
            UpdateState(state);
        }
    }
}
