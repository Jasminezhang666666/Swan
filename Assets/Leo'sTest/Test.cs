using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class Test : MonoBehaviour
{
    public AK.Wwise.Event musicEvent;
    private uint playingID;

    // Start is called before the first frame update
    void Start()
    {
        playingID = musicEvent.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncBeat | AkCallbackType.AK_MusicSyncUserCue | AkCallbackType.AK_MusicSyncAll), MusicCallback);
    }

    // 音乐回调函数
    void MusicCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_info is AkMusicSyncCallbackInfo syncInfo)
        {
            if (in_type == AkCallbackType.AK_MusicSyncUserCue)
            {
                string cueName = syncInfo.userCueName;
                if (cueName == "t")
                {
                    // 执行停止事件的操作，3秒淡出时间
                    AkSoundEngine.ExecuteActionOnEvent(musicEvent.Name, AkActionOnEventType.AkActionOnEventType_Stop, gameObject, 3000);
                }
            }
        }
    }
}
