/*using UnityEngine;
using UnityEngine.Video;

public class Intro_FilmManager : MonoBehaviour
{
    public GameObject[] videoObjects; // Array of GameObjects with VideoPlayers to be assigned in the editor.
    private int currentVideoIndex = 0;

    void Start()
    {
        // Ensure the array has elements.
        if (videoObjects.Length > 0)
        {
            PlayVideo(currentVideoIndex);
        }
    }

    void Update()
    {
        // Check for space key press and ensure there are more videos to play.
        if (Input.GetKeyDown(KeyCode.Space) && currentVideoIndex < videoObjects.Length - 1)
        {
            currentVideoIndex++;
            PlayVideo(currentVideoIndex);
        }
    }

    void PlayVideo(int index)
    {
        // Enable the next GameObject at the specified index.
        videoObjects[index].SetActive(true);
        VideoPlayer videoPlayer = videoObjects[index].GetComponent<VideoPlayer>();

        // Set alpha of all VideoPlayers to 0 except the current one.
        for (int i = 0; i < videoObjects.Length; i++)
        {
            VideoPlayer vp = videoObjects[i].GetComponent<VideoPlayer>();
            if (vp != null)
            {
                vp.targetCameraAlpha = (i == index) ? 1.0f : 0.0f;
            }
        }

        // Play the video if the VideoPlayer component exists.
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}
*/
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro_FilmManager : MonoBehaviour
{
    public VideoPlayer[] videoPlayers; // Array of video players to be assigned in the editor.
    private int currentVideoIndex = 0;

    void Start()
    {
        foreach(VideoPlayer video in videoPlayers)
        {
            video.Play();
            video.Pause();
        }

        if (videoPlayers.Length > 0)
        {
            PlayVideo(currentVideoIndex);
        }
    }

    void Update()
    {
        // Check for space key press and ensure there are more videos to play.
        if (Input.GetKeyDown(KeyCode.Space) && currentVideoIndex < videoPlayers.Length - 1)
        {
            currentVideoIndex++;
            PlayVideo(currentVideoIndex);
            
        }
    }

    void PlayVideo(int index)
    {

        // Play the video at the specified index.
        //videoPlayers[index].Play();
        

        

        videoPlayers[index].Play();
        

        // Attach a listener for when the video finishes.
        //videoPlayers[index].loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Check if the current video is the last in the array.
        if (currentVideoIndex == videoPlayers.Length - 1)
        {
            // Load the next scene when the last video finishes.
            SceneManager.LoadScene("02_Scene1");
        }
    }
}
