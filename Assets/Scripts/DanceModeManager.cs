using AK;
using AK.Wwise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using AKEvent = AK.Wwise.Event;
using Fungus;
using System.Linq;


public class DanceModeManager : MonoBehaviour
{
    public static DanceModeManager instance;
    public bool InDanceMode => _inDanceMode;


    [Header("Rhythm Settings")]
    public string DanceModeFileName = "DanceRhythm.csv";
    public List<Rhythm> CorrectRhythm; //List of all rhythm
    public List<string> tempRhythm; //List of remain possible rhythm
    public string currentRhythm; //player input

    [Header("Click Settings")]
    float reflectTime = 0.1f; //0.1s mouse input detection for human reflection
    float clickTimer = 0f;
    bool leftClicked = false;
    bool rightClicked = false;


    // ***********************************************************************************
    [Header("Light Settings")]
    [SerializeField] private float danceLightIntensity = 0.05f;
    private float _originalLightIntensity;
    private Light2D _globalLight;

    [Header("UI Indicator")]
    [Tooltip("UI element (RectTransform) that shrinks each beat")]
    [SerializeField] private List<RectTransform> beatIndicators;
    private int _currentIndicatorIndex;
    private List<Vector3> _initialScales;

    [Tooltip("Duration of one beat cycle in seconds")]
    [SerializeField] private float beatDuration = 1.36f;
    [Tooltip("Delay before the first beat indicator starts shrinking")]
    [SerializeField] private float beatStartOffset = -0.6f;
    [Tooltip("Smallest scale factor relative to the original")]
    [Range(0f, 1f)]
    [SerializeField] private float minScaleFactor = 0.426f;
    [Tooltip("Speed multiplier for the shrink animation")]
    [SerializeField] private float shrinkSpeed = 1.2f;
    [Tooltip("Time window around beat (in seconds) to accept input")]
    [SerializeField] private float inputBuffer = 0.2f;

    [Header("Full Dance-Mode UI")]
    [Tooltip("Root Canvas (or panel) for all dance-mode UI elements")]
    [SerializeField] private Canvas danceModeCanvas;

    [Header("Wwise Music Event")]
    [Tooltip("Assign your '44' music event here")]
    [SerializeField] private AKEvent Snd_44;
    private uint _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;

    //private DanceMode _danceMode;
    private bool _inDanceMode;
    private float _beatTimer;
    private bool _canAcceptInput;
    private Vector3 _initialIndicatorScale;
    private Player _player;
    private GameObject _playerLightingChild;

    private void Awake()
    {
        instance = this;
        //_danceMode = GetComponent<DanceMode>();
    }

    private void Start()
    {

        LoadRhythm(); // load file
        ResetInputRhythm(); //reset

        //******************************************************************
        // find the global 2D light
        foreach (var l in FindObjectsOfType<Light2D>())
        {
            if (l.lightType == Light2D.LightType.Global)
            {
                _globalLight = l;
                _originalLightIntensity = l.intensity;
                break;
            }
        }

        // disable dance-mode components by default
        //_danceMode.enabled = false;

        // prepare indicators
        _initialScales = new List<Vector3>();
        foreach (var ind in beatIndicators)
        {
            ind.gameObject.SetActive(false);
            _initialScales.Add(ind.localScale);
        }

        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1) if Fungus is in the middle of dialogue, bail out
            if (IsFungusSpeaking())
                return;

            // 2) otherwise toggle dance mode
            if (!_inDanceMode) EnterDanceMode();
            else ExitDanceMode();
        }

        if (_inDanceMode)
        {
            UpdateBeatIndicator();
            GetInputRhythm();
            CheckInputRhythm();
        }
    }


    private void EnterDanceMode()
    {
        _inDanceMode = true;
        DisablePlayer();
        if (_globalLight != null)
            _globalLight.intensity = danceLightIntensity;
        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(true);

        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
        _beatTimer = -beatStartOffset;
        _musicPlayingID = Snd_44.Post(gameObject);
    }

    private void ExitDanceMode()
    {
        _inDanceMode = false;
        RestorePlayer();
        if (_globalLight != null)
            _globalLight.intensity = _originalLightIntensity;
        HideAllIndicators();
        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(false);
        if (_musicPlayingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID);
            _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
    }

    private void UpdateBeatIndicator()
    {
        _beatTimer += Time.deltaTime;
        if (_beatTimer > beatDuration)
            _beatTimer -= beatDuration;

        float timer = Mathf.Max(0, _beatTimer);
        float t = Mathf.Clamp01((timer * shrinkSpeed) / beatDuration);
        float scale = Mathf.Lerp(1f, minScaleFactor, t);

        var ind = beatIndicators[_currentIndicatorIndex];
        ind.localScale = _initialScales[_currentIndicatorIndex] * scale;
        _canAcceptInput = timer <= inputBuffer || (beatDuration - timer) <= inputBuffer;
    }

    private void AdvanceIndicator()
    {
        HideCurrentIndicator();
        _currentIndicatorIndex++;
        if (_currentIndicatorIndex >= beatIndicators.Count)
        {
            Debug.Log("Dance sequence complete!");
            ExitDanceMode();
        }
        else
        {
            ResetInputRhythm();
            ShowCurrentIndicator();
            _beatTimer = -beatStartOffset;
        }
    }

    private void ResetSequence()
    {
        ResetInputRhythm();
        HideAllIndicators();
        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
        _beatTimer = -beatStartOffset;
    }

    private void ShowCurrentIndicator()
    {
        var ind = beatIndicators[_currentIndicatorIndex];
        ind.gameObject.SetActive(true);
        ind.localScale = _initialScales[_currentIndicatorIndex];
    }

    private void HideCurrentIndicator()
    {
        beatIndicators[_currentIndicatorIndex].gameObject.SetActive(false);
    }

    private void HideAllIndicators()
    {
        foreach (var ind in beatIndicators)
            ind.gameObject.SetActive(false);
    }

    /*
    private void HandleBeatInput()
    {
        if (!_danceMode.enabled) return;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (_canAcceptInput)
            {
                Debug.Log("Correct beat!");
            }
            else Debug.Log("Missed beat!");
        }
    }
    */

    //******************************************************************************

    /// <summary>
    /// load correct rhythm from file DanceMode
    /// </summary>
    void LoadRhythm()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, DanceModeFileName);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            // skip title row
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(',');

                if (parts.Length >= 2)
                {
                    Rhythm r = new Rhythm();
                    r.rhythmID = parts[0].Trim();
                    r.rhythmName = parts[1].Trim();
                    CorrectRhythm.Add(r);
                }
            }
        }
        else
        {
            Debug.LogError("File not found at path: " + filePath);
        }
    }

    /// <summary>
    /// Reset input to empty and temp to original
    /// </summary>
    void ResetInputRhythm()
    {
        tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
        currentRhythm = string.Empty; //clean player input
    }

    /// <summary>
    /// Get player input rhythm from mouse
    /// </summary>
    private void GetInputRhythm()
    {
        if (Input.GetMouseButtonDown(0)) leftClicked = true;
        if (Input.GetMouseButtonDown(1)) rightClicked = true;

        if (clickTimer > 0f)
        {
            if (leftClicked || rightClicked)
                clickTimer -= Time.deltaTime;
        }
        else
        {
            if (leftClicked && rightClicked)
                currentRhythm += "2";
            else if (leftClicked)
                currentRhythm += "0";
            else if (rightClicked)
                currentRhythm += "1";

            tempRhythm = tempRhythm.Where(s => s.StartsWith(currentRhythm)).ToList();
            leftClicked = rightClicked = false;
            clickTimer = reflectTime;

            if (_canAcceptInput)
            {
                Debug.Log("Correct beat!");
                AdvanceIndicator();
            }
            else
            {
                Debug.Log("Missed beat!");
                ResetSequence();
            }
        }
    }
    /// <summary>
    /// check whether current input rhythm match correct rhythm
    /// </summary>
    void CheckInputRhythm()
    {
        if (tempRhythm.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (CorrectRhythm.Exists(r => r.rhythmID == currentRhythm))
                {
                    Rhythm result = CorrectRhythm.Find(r => r.rhythmID == currentRhythm);
                    Debug.Log(result.rhythmName);
                    ResetInputRhythm();
                }
                else
                {
                    Debug.Log("Wrong!!!!!!!!");
                    ResetInputRhythm();
                }
            }
        }
        else
        {
            Debug.Log("Wrong!!!!!!!!");
            ResetInputRhythm();
        }
    }

    /// <summary>
    /// Returns true if any Flowchart is mid-dialogue (i.e. has executing blocks)
    /// </summary>
    private bool IsFungusSpeaking()
    {
        // You can cache this array if you want, but it’s cheap enough for most cases
        return FindObjectsOfType<Flowchart>().Any(fc => fc.HasExecutingBlocks());
    }

    private void DisablePlayer()
    {
        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) _player = go.GetComponent<Player>();
        }
        if (_player != null)
        {
            _player.canMove = false;
            _player.enabled = false;
            var rb = _player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
            var anim = _player.GetComponent<Animator>();
            if (anim != null) anim.SetBool("isMoving", false);
            var sr = _player.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting")) child.gameObject.SetActive(false);
                else
                {
                    _playerLightingChild = child.gameObject;
                    _playerLightingChild.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogError("DanceModeManager: No Player tagged 'Player' found.");
        }
    }

    private void RestorePlayer()
    {
        if (_player != null)
        {
            _player.enabled = true;
            _player.canMove = true;
            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting")) child.gameObject.SetActive(true);
                else if (_playerLightingChild != null)
                    _playerLightingChild.SetActive(false);
            }
        }
    }

}

[Serializable]
public struct Rhythm
{
    public string rhythmName;
    public string rhythmID; //correct input

}
