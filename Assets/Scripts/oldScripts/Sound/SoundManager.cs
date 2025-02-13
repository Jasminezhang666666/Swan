using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] bgm;

    public static SoundManager instance { get; private set; }

    void Awake()
    {
        #region Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            Debug.Log("Audio Source Missing");
            Debug.Assert(false);
        }
    }

    public void PlaySong(int bgmIndx)
    {
        audioSource.clip = bgm[bgmIndx];
        audioSource.Play();
    }

    private void Update()
    {
        //print("Audio still here");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {//change sound when scene load
        print("On Scene Loaded: " + scene.name);

        if (scene.name == "StarAnim" || scene.name == "EndAnim")
        {
            PlaySong(2);
        }
        else if (scene.name == "Level_1")
        {
            PlaySong(1);
        }
    }
}

