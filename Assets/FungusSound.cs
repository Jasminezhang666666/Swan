using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using UnityEngine.SceneManagement;

public class FungusSound : MonoBehaviour
{
    [Header("Introduction Sounds")]
    public AK.Wwise.Event Snd_NotesDown;
    public AK.Wwise.Event Snd_BrushItOff;
    public AK.Wwise.Event Snd_IntroBackground;

    // Use uint for the playing ID
    private uint backgroundPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;

    private void Awake()
    {
        // Make sure this object persists across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + sceneName);
        if (sceneName == "02_Scene1")
        {
            PlayBackground();
        }
        else if (sceneName == "Rm_Hallway01")
        {
            StopBackgroundMusic();
        }
    }

    public void PlayBackground()
    {
        backgroundPlayingID = Snd_IntroBackground.Post(this.gameObject);
        Debug.Log("Playing background music, ID: " + backgroundPlayingID);
    }

    public void StopBackgroundMusic()
    {
        if (backgroundPlayingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(backgroundPlayingID);
            Debug.Log("Stopped background music, ID: " + backgroundPlayingID);
            backgroundPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
        else
        {
            Debug.Log("No valid background music playing to stop.");
        }
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
