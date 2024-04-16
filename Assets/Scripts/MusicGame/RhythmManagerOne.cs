using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RhythmManagerOne : MonoBehaviour
{

    [SerializeField] AK.Wwise.Event musicEvent;
    uint playingID;

    [SerializeField] float beatDuration;
    [SerializeField] float barDuration;
    [SerializeField] bool durationSet = false;


    #region Additional Parameters
    [SerializeField] SpawnObjectOne spawnObject;

    [SerializeField] Transform spawnLocation;

    public int OkWindowMillis = 200;
    public int GoodWindowMillis = 100;
    public int PerfectWindowMillis = 50;

    public List<SpawnObjectOne> spawnObjectList = new List<SpawnObjectOne>();

    #endregion

    #region Setup
    private void Start()
    {
        playingID = musicEvent.Post(gameObject, (uint)(AkCallbackType.AK_MusicSyncAll | AkCallbackType.AK_EnableGetMusicPlayPosition | AkCallbackType.AK_MusicSyncUserCue | AkCallbackType.AK_MIDIEvent), CallbackFunction);
    }
    #endregion

    void Update()
    {

    }


    void CallbackFunction(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        AkMusicSyncCallbackInfo musicInfo;

        if (in_info is AkMusicSyncCallbackInfo)
        {
            musicInfo = (AkMusicSyncCallbackInfo)in_info;
            switch (in_type)
            {

                case AkCallbackType.AK_MusicSyncUserCue:
                    ManageUserCue(musicInfo.userCueName);
                    break;

                case AkCallbackType.AK_MusicSyncBeat:
                    //HERE IS WHERE YOU CAN DO SOMETHING ON THE BEAT
                    //OnTheBeat();
                    break;

                case AkCallbackType.AK_MusicSyncBar:
                    //HERE IS WHERE YOU CAN DO SOMETHING ON THE BAR
                    //OnTheBar();
                    break;
            }

            if (in_type is AkCallbackType.AK_MusicSyncBar)
            {
                if (!durationSet)
                {
                    beatDuration = musicInfo.segmentInfo_fBeatDuration;
                    barDuration = musicInfo.segmentInfo_fBarDuration;
                    durationSet = true;
                }
            }
        }
    }
    void ManageUserCue(string s)
    {
        switch (s)
        {
            case "A1":
                Debug.Log("This is cue A1, it spawns from channel A and lasts until A2 is triggered");
                MusicGameManager.Instance.SpawnNodeA();
                break;
            case "A2":
                Debug.Log("This is cue A2, it marks the end of a note from channel A ");
                MusicGameManager.Instance.StopSpawnNodeA();
                break;
            case "B1":
                Debug.Log("This is cue B1");
                MusicGameManager.Instance.SpawnNodeB();
                break;
            case "B2":
                Debug.Log("This is cue B2");
                MusicGameManager.Instance.StopSpawnNodeB();
                break;
            case "C1":
                Debug.Log("This is cue C1");
                MusicGameManager.Instance.SpawnNodeC();
                break;
            case "C2":
                Debug.Log("This is cue C2");
                MusicGameManager.Instance.StopSpawnNodeC();
                break;
            case "D1":
                Debug.Log("This is cue D1");
                MusicGameManager.Instance.SpawnNodeD();
                break;
            case "D2":
                Debug.Log("This is cue D2");
                MusicGameManager.Instance.StopSpawnNodeD();
                break;
            case "E1":
                Debug.Log("This is cue E1");
                MusicGameManager.Instance.SpawnNodeE();
                break;
            case "E2":
                Debug.Log("This is cue E2");
                MusicGameManager.Instance.StopSpawnNodeE();
                break;
        }
    }

    /// <summary>
    /// Here is an example function of a place where you can do something that reacts on the beat
    /// </summary>
    void OnTheBeat()
    {
        Debug.Log("Here is a beat event!");
    }

    /// <summary>
    /// Here is an example function of a place where you can do something that reacts on the bar
    /// </summary>
    void OnTheBar()
    {
        Debug.Log("Here is a bar event!");
    }


    #region Other
    public int GetMusicTimeInMS()
    {
        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);
        return segmentInfo.iCurrentPosition;
    }

    //We're going to call this when we spawn a gem, in order to determine when it's crossing time should be
    //Crossing time is based on the current playback position, our beat duration, and our beat offset
    public int SetCrossingTimeInMS(int beatOffset)
    {
        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(playingID, segmentInfo, true);
        int offsetTime = Mathf.RoundToInt(1000 * beatDuration * beatOffset);
        //Debug.Log($"Crossing time: {segmentInfo.iCurrentPosition + offsetTime}");
        return segmentInfo.iCurrentPosition + offsetTime;
    }

    void SpawnItem(AkMusicSyncCallbackInfo info)
    {
        int spawnTime = GetMusicTimeInMS();
        SpawnObjectOne s = Instantiate(spawnObject, new Vector3(spawnLocation.position.x, 0.0f, spawnLocation.position.z), Quaternion.identity);
        s.SetSpawnItemInfo(this, spawnTime, SetCrossingTimeInMS(1));
        s.name = $"{s.name} | {spawnTime}";
        spawnObjectList.Insert(0, s);
    }
    #endregion

}

public enum HitTiming { None, Perfect, Good, Okay, Late, Miss }