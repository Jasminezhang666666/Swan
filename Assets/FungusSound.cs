using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class FungusSound : MonoBehaviour
{
    [Header("Introduction Sounds")]
    public AK.Wwise.Event Snd_NotesDown;
    public AK.Wwise.Event Snd_BrushItOff;
    public AK.Wwise.Event Snd_IntroBackground;


    //Introduction
    public void PlayBackground()
    {
        Snd_IntroBackground.Post(this.gameObject);
    }

    public void PlayNotesDown()
    {
        Snd_NotesDown.Post(this.gameObject);
    }

    public void BrushItOff()
    {
        Snd_BrushItOff.Post(this.gameObject);
    }
}
