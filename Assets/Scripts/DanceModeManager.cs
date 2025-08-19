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

    [Header("Result Display")]
    [Tooltip("The UI Image whose color we will tint on a full‐combo success (else it goes black)")]
    [SerializeField] private Image resultImage;

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

    [Tooltip("Beat per Minute")]
    [SerializeField] private float BPM = 1.36f;
    private float beatDuration;
    [Tooltip("Delay before the first beat indicator starts shrinking")]
    [SerializeField] private float beatStartOffset = -0.6f;
    [Tooltip("Smallest scale factor relative to the original")]
    [SerializeField] private float maxScaleFactor = 40f;
    [Tooltip("Speed multiplier for the scale animation")]
    [SerializeField] private float scaleSpeed = 1.2f;
    [Tooltip("Time window around beat (in seconds) to accept input")]
    [SerializeField] private float inputBuffer = 0.2f;

    [Header("Full Dance-Mode UI")]
    [Tooltip("Root Canvas (or panel) for all dance-mode UI elements")]
    [SerializeField] private Canvas danceModeCanvas;

    [Header("Wwise Music Event")]
    [Tooltip("Assign your '44' music event here")]
    [SerializeField] private AKEvent Snd_44;
    private uint _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
    [Tooltip("Assign your background music event here")]
    [SerializeField] private AKEvent BackgroundMusic;
    private uint _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;

    [Header("Result Rectangles")]
    [Tooltip("A simple UI Image prefab (e.g. an empty Image with a white square sprite)")]
    [SerializeField] private Image rectanglePrefab;
    [Tooltip("Parent RectTransform where new rectangles will be placed")]
    [SerializeField] private RectTransform rectanglesParent;
    [Tooltip("Horizontal distance between successive rectangles")]
    [SerializeField] private float rectangleSpacing = 50f;
    [Tooltip("X‑position of the first rectangle")]
    private float rectangleStartX = -350f;
    [Tooltip("Y‑position of the first rectangle")]
    private float rectangleStartY = 0f;

    [Header("Player Light (optional)")]
    [SerializeField] private GameObject playerDanceLight; //in regular scenes, don't assign it for dark fog scenes
    // Track which child objects we disabled so we can restore exactly those (and nothing else)
    private readonly List<GameObject> _childrenDisabledByDance = new List<GameObject>();
    // Track the original active state of the optional dance light so we can restore it
    private bool _playerDanceLightWasActive = false;

    [Header("Effects")]
    [SerializeField] private GameObject lightBallPrefab; // assign your prefab with DanceEffect_LightBall



    // runtime list of spawned rectangles
    private List<Image> _resultRectangles = new List<Image>();

    [Tooltip("Any UI GameObject you want to hide if you miss a beat after having spawned at least one rectangle")]
    [SerializeField] private GameObject uiDisappearObject; //dance DONE!
    private bool _inputReceivedThisBeat;


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

        if (resultImage != null)
            resultImage.color = Color.white;
    }

    private void Update()
    {
        beatDuration = 60 / BPM;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsFungusSpeaking())
                return;

            if (!_inDanceMode)
            {
                EnterDanceMode();
            }
            else
            {
                ResetDanceMode(false); 
                ExitDanceMode();
            }
        }


        if (_inDanceMode)
        {
            UpdateBeatIndicator();
            GetInputRhythm();
            CheckInputRhythm();
        }

        // TEST ONLY!!!!!!!!: Press L to spawn a shrinking light on the player
        //触发发光效果
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnPlayerFadeLight(localOffset: Vector3.zero); 
        }

    }


    private void EnterDanceMode()
    {
        _musicPlayingID_BG = BackgroundMusic.Post(gameObject); //play background music
        _inputReceivedThisBeat = false;
        _inDanceMode = true;
        DisablePlayer();
        if (_globalLight != null)
            _globalLight.intensity = danceLightIntensity;
        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(true);

        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
        _beatTimer = -beatStartOffset;
        //_musicPlayingID = Snd_44.Post(gameObject);
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
        //and background music
        if (_musicPlayingID_BG != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID_BG);
            _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
    }

    private void UpdateBeatIndicator()
    {
        // 1) Do we even have any indicators assigned?
        if (beatIndicators == null || beatIndicators.Count == 0)
        {
            Debug.LogError("DanceModeManager: beatIndicators list is null or empty!  Assign your UI RectTransforms in the Inspector.");
            return;
        }

        // 2) Have we initialized _initialScales, and does it match?
        if (_initialScales == null || _initialScales.Count != beatIndicators.Count)
        {
            Debug.LogError("DanceModeManager: _initialScales isn't set up or doesn't match beatIndicators.Count.");
            return;
        }

        // 3) Is our current index valid?
        if (_currentIndicatorIndex < 0 || _currentIndicatorIndex >= beatIndicators.Count)
        {
            Debug.LogError($"DanceModeManager: _currentIndicatorIndex {_currentIndicatorIndex} out of range 0..{beatIndicators.Count - 1}");
            return;
        }

        // ——— safe to run your normal code now ———
        _beatTimer += Time.deltaTime;
        if (_beatTimer > beatDuration)
        {
            if (!_inputReceivedThisBeat && _resultRectangles.Count > 0)
            {
                EndDanceEarly();
                return;
            }

            // otherwise, proceed as normal:
            _inputReceivedThisBeat = false;
            _beatTimer -= beatDuration;
            Snd_44.Post(gameObject);
        }



        float timer = Mathf.Max(0, _beatTimer);
        float t = Mathf.Clamp01((timer * scaleSpeed) / beatDuration);
        float scale = Mathf.Lerp(1f, maxScaleFactor, t);

        var ind = beatIndicators[_currentIndicatorIndex];
        ind.localScale = _initialScales[_currentIndicatorIndex] * scale;
        _canAcceptInput = timer <= inputBuffer || (beatDuration - timer) <= inputBuffer;
    }


    private void AdvanceIndicator()
    {
        HideCurrentIndicator();
        _currentIndicatorIndex++;

        // Check if we've reached the end of the rhythm sequence
        if (_currentIndicatorIndex >= beatIndicators.Count)
        {
            Debug.Log("Dance sequence complete!");

            // —— FULL COMBO: tint the image to the matched hex color ——
            if (resultImage != null)
            {
                // right after you hit full combo:
                Debug.Log($"[Advance] currentRhythm = '{currentRhythm}'");

                var match = CorrectRhythm
                    .FirstOrDefault(r => r.rhythmID == currentRhythm);

                if (string.IsNullOrEmpty(match.rhythmID))
                {
                    Debug.LogError($"No rhythm entry for ID '{currentRhythm}'");
                    resultImage.color = Color.black;
                }
                else if (!ColorUtility.TryParseHtmlString(match.colorHex, out var c))
                {
                    Debug.LogError($"Failed to parse color '{match.colorHex}'");
                    resultImage.color = Color.black;
                }
                else
                {
                    Debug.Log($"✅ Applying color {match.colorHex} for rhythm '{match.rhythmID}'");
                    resultImage.color = c;

                    // spawn the new rectangle
                    var img = Instantiate(rectanglePrefab, rectanglesParent);
                    img.color = c;
                    var rt = img.rectTransform;
                    // position using both startX and startY
                    rt.anchoredPosition = new Vector2(
                        rectangleStartX + _resultRectangles.Count * rectangleSpacing,
                        rectangleStartY
                    );
                    _resultRectangles.Add(img);
                }
            }

            // reset indicators for the next round, preserving the color tint
            tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
            currentRhythm = string.Empty;
            HideAllIndicators();
            _currentIndicatorIndex = 0;
            ShowCurrentIndicator();
            _beatTimer = Mathf.Min(_beatTimer, beatDuration * 0.8f);
            return;
        }
        else
        {
            // still in the middle of a combo: advance the indicator but keep currentRhythm intact
            ShowCurrentIndicator();
            _beatTimer = Mathf.Min(_beatTimer, beatDuration * 0.8f);
        }
    }


    private void ResetSequence()
    {
        _inputReceivedThisBeat = false;

        // —— on any miss, reset image to black ——
        if (resultImage != null)
            resultImage.color = Color.black;

        tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
        currentRhythm = string.Empty;

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

    //******************************************************************************

    /// <summary>
    /// load correct rhythm from file DanceMode
    /// </summary>
    void LoadRhythm()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, DanceModeFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found at path: " + filePath);
            return;
        }

        var lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length >= 2)
            {
                // strip whitespace *and* any BOM (U+FEFF) just in case
                string id = parts[0].Trim().Trim('\uFEFF');
                string hex = (parts.Length >= 3 ? parts[2] : "#ffffff").Trim();

                var r = new Rhythm
                {
                    rhythmID = id,
                    rhythmName = parts[1].Trim(),
                    colorHex = hex
                };

                CorrectRhythm.Add(r);
            }

            else Debug.LogWarning($"Line {i + 1} malformed: need 3 columns");
        }

        Debug.Log($"[LoadRhythm] found {CorrectRhythm.Count} rhythms:");
        foreach (var x in CorrectRhythm)
            Debug.Log($"    ID='{x.rhythmID}', hex='{x.colorHex}'");

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
    void GetInputRhythm()
    {
        // record click flags
        if (Input.GetMouseButtonDown(0)) leftClicked = true;
        if (Input.GetMouseButtonDown(1)) rightClicked = true;

        // still in the reflect window?
        if (clickTimer > 0f)
        {
            if (leftClicked || rightClicked)
                clickTimer -= Time.deltaTime;
        }
        // only *if* we just clicked do we consume input
        else if (leftClicked || rightClicked)
        {
            _inputReceivedThisBeat = true;

            // build your rhythm string…
            if (leftClicked && rightClicked)
            {
                currentRhythm += "2";
                print("2");
            }
            else if (leftClicked)
            {
                currentRhythm += "0";
                print("0");
            }
            else if (rightClicked)
            {
                currentRhythm += "1";
                print("1");
            }

            // filter, clear flags, reset the timer
            tempRhythm = tempRhythm.Where(s => s.StartsWith(currentRhythm)).ToList();
            leftClicked = rightClicked = false;
            clickTimer = reflectTime;

            // beat check
            if (_canAcceptInput)
            {
                //Debug.Log("Correct beat!");
                AdvanceIndicator();
            }
            else
            {
                /*测试用！！！！！记得改回来
                Debug.Log("Correct beat!");
                AdvanceIndicator();
                */
                Debug.Log("Missed beat!");
                var cam = Camera.main.GetComponent<Chapter1_Camera>();
                if (cam != null) cam.ShakeCamera();
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

            _childrenDisabledByDance.Clear();

            if (playerDanceLight != null)
                _playerDanceLightWasActive = playerDanceLight.activeSelf;

            foreach (Transform child in _player.transform)
            {
                var childGo = child.gameObject;

                // 1) never touch the assigned dance light
                if (playerDanceLight != null && childGo == playerDanceLight)
                    continue;

                // 2) ignore any other lighting objects entirely (do NOT turn on/off)
                if (child.CompareTag("Lighting"))
                    continue;

                // 3) normal children: disable and remember
                if (childGo.activeSelf)
                {
                    childGo.SetActive(false);
                    _childrenDisabledByDance.Add(childGo);
                }
            }

            // ensure the assigned dance light is ON during dance mode
            if (playerDanceLight != null)
                playerDanceLight.SetActive(true);
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

            if (_childrenDisabledByDance.Count > 0)
            {
                foreach (var go in _childrenDisabledByDance)
                {
                    if (go != null) go.SetActive(true);
                }
                _childrenDisabledByDance.Clear();
            }

            if (playerDanceLight != null)
                playerDanceLight.SetActive(_playerDanceLightWasActive);
        }
    }




    private void EndDanceEarly()
    {
        // stop the Wwise music beat
        if (_musicPlayingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID);
            _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
        //and background music
        if (_musicPlayingID_BG != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID_BG);
            _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }

        // optionally restore lights & player
        if (_globalLight != null)
            _globalLight.intensity = _originalLightIntensity;

        if (uiDisappearObject != null)
            uiDisappearObject.SetActive(false);

        foreach (Transform child in _player.transform)
        {
            if (child.CompareTag("Lighting")) child.gameObject.SetActive(false);
        }

        //PLAY player dance animation here
    }

    /// <summary>
    /// Hard reset of the dance round:
    /// - clears saved color rectangles
    /// - clears inputs and tempRhythm
    /// - resets indicators/timers/flags
    /// - restarts the background music from the beginning
    /// </summary>
    private void ResetDanceMode(bool restartMusicNow)
    {
        // input & flags
        _inputReceivedThisBeat = false;
        leftClicked = rightClicked = false;
        clickTimer = 0f;
        _canAcceptInput = false;

        // rhythm buffers
        tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
        currentRhythm = string.Empty;

        // UI: result color back to black (fresh game)
        if (resultImage != null)
            resultImage.color = Color.black;

        // UI: remove every previously spawned rectangle
        if (_resultRectangles != null && _resultRectangles.Count > 0)
        {
            for (int i = _resultRectangles.Count - 1; i >= 0; i--)
            {
                if (_resultRectangles[i] != null)
                    Destroy(_resultRectangles[i].gameObject);
            }
            _resultRectangles.Clear();
        }

        // UI: restore any object that should be visible at the start
        if (uiDisappearObject != null)
            uiDisappearObject.SetActive(true);

        // Indicators back to initial state
        HideAllIndicators();
        if (_initialScales != null && _initialScales.Count == beatIndicators.Count)
        {
            for (int i = 0; i < beatIndicators.Count; i++)
                beatIndicators[i].localScale = _initialScales[i];
        }
        _currentIndicatorIndex = 0;
        _beatTimer = -beatStartOffset;  // align to first beat
        if (beatIndicators != null && beatIndicators.Count > 0)
            ShowCurrentIndicator();

        // Music: restart BG immediately
        if (restartMusicNow)
        {
            if (_musicPlayingID_BG != AkSoundEngine.AK_INVALID_PLAYING_ID)
            {
                AkSoundEngine.StopPlayingID(_musicPlayingID_BG);
                _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;
            }
            _musicPlayingID_BG = BackgroundMusic.Post(gameObject); // fresh from the start
        }
    }

    /// <summary>
    /// Spawns a Light2D "light ball" prefab as a child of the Player,
    /// using whatever duration is set on the prefab itself.
    /// </summary>
    public GameObject SpawnPlayerFadeLight(GameObject prefab = null, Vector3 localOffset = default)
    {
        var usePrefab = prefab != null ? prefab : lightBallPrefab;
        if (usePrefab == null)
        {
            Debug.LogError("SpawnPlayerFadeLight: no prefab provided and lightBallPrefab is not assigned.");
            return null;
        }

        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) _player = go.GetComponent<Player>();
        }
        if (_player == null)
        {
            Debug.LogError("SpawnPlayerFadeLight: Player not found.");
            return null;
        }

        var inst = Instantiate(usePrefab, _player.transform);
        inst.transform.localPosition = localOffset;
        inst.transform.localRotation = Quaternion.identity;

        var effect = inst.GetComponent<DanceEffect_LightBall>();
        if (effect != null) effect.Begin(); 

        return inst;
    }


}


[Serializable]
public struct Rhythm
{
    public string rhythmName;
    public string rhythmID;    // the 4‐digit key
    public string colorHex;    // e.g. "#ff6b6b"
}
