using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public double[] pauseTimes; // Array to define the times (in seconds) when the video should pause.
    private int currentPauseIndex = 0;

    private void Start()
    {
        // Add 0.1 to each element in pauseTimes before anything else
        for (int i = 0; i < pauseTimes.Length; i++)
        {
            pauseTimes[i] += 0.275;
        }

        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared; // Subscribe to the prepare event.
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play(); // Start playing automatically once the video is prepared.
    }

    private void Update()
    {
        // Print the current time of the video
        if (videoPlayer != null)
        {
            print("Video Time: " + videoPlayer.time);
        }

        // Handle space bar input to play/pause
        if (videoPlayer != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
            else
            {
                videoPlayer.Play();
            }
        }

        // Automatically pause at specified times
        if (videoPlayer.isPlaying && currentPauseIndex < pauseTimes.Length)
        {
            if (videoPlayer.time >= pauseTimes[currentPauseIndex])
            {
                videoPlayer.Pause();
                currentPauseIndex++;
            }
        }
    }
}
