using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R3_light : Items
{

    [Header("Settings")]
    string matchesName = "Matches";
    string normalCandleName = "Candle Unlit";
    string litCandleName = "Candle Lit";

    [Header("Sprites")]
    [SerializeField] Sprite withCandle;
    [SerializeField] Sprite litCandle;
    [SerializeField] Sprite withoutCandle;
    [SerializeField] Sprite withoutCandleHighlight;

    enum LightState
    { // OMG clown!! --Faustine, why were you using ints
        Empty,
        WithCandle,
        LitCandle
    }
    LightState state = LightState.Empty;
    string lightPref = "Light Left";

    protected override void Start()
    {
        base.Start();

        interctDia.Add("Candle holder. You need a candle and fire source to light it up.");

        if (PlayerPrefs.HasKey(lightPref)) // if state already exists
        {
            state = (LightState)PlayerPrefs.GetInt(lightPref); // load saved state
            UpdateState(state);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (onObject && state == LightState.Empty && InventoryStorage.instance.selectedItemId == normalCandleName) // put candle on
        {
            if (InventoryStorage.instance.HasItem(normalCandleName))
            {
                InventoryStorage.instance.RemoveItem(normalCandleName);
            }

            state = LightState.WithCandle;
            UpdateState(state);
        }

        if (onObject && state == LightState.WithCandle && InventoryStorage.instance.selectedItemId == matchesName) // put matches on
        {
            if (InventoryStorage.instance.HasItem(matchesName))
            {
                InventoryStorage.instance.RemoveItem(matchesName);
            }

            state = LightState.LitCandle;
            UpdateState(state);
        }
    }

    private void UpdateState(LightState input)
    {
        state = input;
        PlayerPrefs.SetInt(lightPref, (int)state); // set state

        switch (state)
        {
            case LightState.Empty:
                UpdateSprites(withoutCandle, withoutCandleHighlight);
                break;
            case LightState.WithCandle:
                UpdateSprites(withCandle, withCandle);
                break;
            case LightState.LitCandle:
                UpdateSprites(litCandle, litCandle);
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

        if (state == LightState.LitCandle)
        {
            if (!InventoryStorage.instance.HasItem(litCandleName))
            {
                InventoryStorage.instance.AddItem(litCandleName);
            }

            state = LightState.Empty;
            UpdateState(state);
        }
    }
}
