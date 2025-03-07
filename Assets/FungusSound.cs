using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using UnityEngine.SceneManagement;

public class FungusSound : MonoBehaviour
{
    public static FungusSound Instance { get; private set; }

    [Header("Introduction Sounds")]
    public AK.Wwise.Event Snd_NotesDown;
    public AK.Wwise.Event Snd_BrushItOff;
    public AK.Wwise.Event Snd_IntroBackground;

    private uint backgroundPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name);
        if (scene.name == "02_Interview")
        {
            StartCoroutine(PlayBackgroundWithDelay(1f)); // Wait 1 second before playing
        }
        else if (scene.name == "Rm_Hallway01")
        {
            StopBackgroundMusic();
        }
    }

    private IEnumerator PlayBackgroundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBackground();
    }

    public void PlayBackground()
    {
        backgroundPlayingID = Snd_IntroBackground.Post(this.gameObject);
        Debug.Log("Playing background music, ID: " + backgroundPlayingID);
    }

    public void StopBackgroundMusic()
    {
        AkSoundEngine.ExecuteActionOnEvent(Snd_IntroBackground.Name,
                                             AkActionOnEventType.AkActionOnEventType_Stop,
                                             gameObject,
                                             3000);
        Debug.Log("Stopped background music using ExecuteActionOnEvent.");
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
