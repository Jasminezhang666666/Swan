using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

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

            // 初始化播放第一个视频
            activePlayer.clip = videoClips[currentVideoIndex];
            activePlayer.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextVideo();
        }
    }

    void PlayNextVideo()
    {
        // 计算下一个视频的索引
        currentVideoIndex = (currentVideoIndex + 1) % videoClips.Count;

        // 准备下一个视频
        preparingPlayer.clip = videoClips[currentVideoIndex];
        preparingPlayer.prepareCompleted += OnVideoPrepared;

        Debug.Log(currentVideoIndex);
        preparingPlayer.Prepare(); // 准备新的视频
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared; // 解除绑定事件

        // 开始播放新视频
        preparingPlayer.Play();

        // 交换 activePlayer 和 preparingPlayer
        VideoPlayer temp = activePlayer;
        activePlayer = preparingPlayer;
        preparingPlayer = temp;

        // 停止原来的播放器
        preparingPlayer.Stop();
    }

}
