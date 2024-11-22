using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Add this to manage scene loading

public class FilmManagerNew : MonoBehaviour
{
    public VideoPlayer videoPlayerA;
    public VideoPlayer videoPlayerB;
    public List<VideoClip> videoClips;

    private VideoPlayer activePlayer;
    private VideoPlayer preparingPlayer;
    private int currentVideoIndex = 0;

    void Awake()
    {
        if (videoClips.Count > 0)
        {
            activePlayer = videoPlayerA;
            preparingPlayer = videoPlayerB;

            // Initialize and play the first video
            activePlayer.clip = videoClips[currentVideoIndex];
            activePlayer.Play();

            // Subscribe to the loopPointReached event
            activePlayer.loopPointReached += OnVideoFinished;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Only allow Space bar to function if not on the last video
            if (currentVideoIndex < videoClips.Count - 1)
            {
                PlayNextVideo();
            }
            else
            {
                // Optional: Provide feedback that no more videos are available
                Debug.Log("No more videos to play.");
            }
        }
    }

    void PlayNextVideo()
    {
        // Increment the video index if not at the last video
        if (currentVideoIndex < videoClips.Count - 1)
        {
            currentVideoIndex++;

            // Prepare the next video
            preparingPlayer.clip = videoClips[currentVideoIndex];
            preparingPlayer.prepareCompleted += OnVideoPrepared;

            Debug.Log($"Preparing video index: {currentVideoIndex}");
            preparingPlayer.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared; // Unsubscribe from the event

        // Swap active and preparing players
        VideoPlayer temp = activePlayer;
        activePlayer = preparingPlayer;
        preparingPlayer = temp;

        // Unsubscribe from the previous player's loopPointReached event
        preparingPlayer.loopPointReached -= OnVideoFinished;

        // Start playing the new active player
        activePlayer.Play();

        // Subscribe to the loopPointReached event for the new active player
        activePlayer.loopPointReached += OnVideoFinished;

        // Stop the previous player
        preparingPlayer.Stop();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (currentVideoIndex == videoClips.Count - 1)
        {
            // Load the specified scene when the last video finishes
            SceneManager.LoadScene("02_Scene1");
        }
        else
        {
            // Optional: Prepare the next video automatically
            // PlayNextVideo();
        }
    }
}
