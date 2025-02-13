using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Rm3_blackShoes : Items
{

    [Header("Sprites")]
    [SerializeField] Sprite blackShoesNoMatches;
    string matchesName = "Matches";

    enum ShoeState
    { // OMG clown!! --Faustine, why were you using ints
        Matches,
        NoMatches
    }
    ShoeState state = ShoeState.Matches;
    string matchesPref = "MatchesPref";

    protected override void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey(matchesPref)) // if state already exists
        {
            state = (ShoeState)PlayerPrefs.GetInt(matchesPref); // load saved state
            UpdateState(state);
        }

        interctDia.Add("You found matches in the shoes.");
    }

    private void UpdateState(ShoeState input)
    {
        state = input;
        PlayerPrefs.SetInt(matchesPref, (int)state); // set state

        switch (state)
        {
            case ShoeState.Matches:
                break;
            case ShoeState.NoMatches:
                UpdateSprites(blackShoesNoMatches, blackShoesNoMatches);
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

        if (state == ShoeState.Matches)
        {
            if (!InventoryStorage.instance.HasItem(matchesName))
            {
                InventoryStorage.instance.AddItem(matchesName);
            }

            state = ShoeState.NoMatches;
            UpdateState(state);
        }
    }
}