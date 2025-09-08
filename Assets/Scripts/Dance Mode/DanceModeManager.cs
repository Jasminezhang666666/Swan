using AK;
using AK.Wwise;
using AKEvent = AK.Wwise.Event;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Fungus;

public class DanceModeManager : MonoBehaviour
{
    public static DanceModeManager instance;
    public bool InDanceMode => _inDanceMode;

    [Header("Timing")]
    [Tooltip("Beats per minute of the click/metronome")]
    [SerializeField] private float BPM = 1.36f;
    [Tooltip("Delay before first indicator starts shrinking (sec)")]
    [SerializeField] private float beatStartOffset = -0.6f;
    [Tooltip("Time window around beat to accept input (sec)")]
    [SerializeField] private float inputBuffer = 0.2f;

    [Header("Co-Components")]
    [SerializeField] private DanceModeUI ui;
    [SerializeField] private RhythmChecker checker;

    [Header("Scene Lighting")]
    [SerializeField] private float danceLightIntensity = 0.05f;
    private float _originalLightIntensity;
    private Light2D _globalLight;

    [Header("Audio (Wwise)")]
    [Tooltip("Beat SFX (previously Snd_44)")]
    [SerializeField] private AKEvent BeatEvent;
    [Tooltip("Background music during dance mode")]
    [SerializeField] private AKEvent BackgroundMusic;
    private uint _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;

    [Header("Player & Effects")]
    [SerializeField] private GameObject playerDanceLight;
    [SerializeField] private GameObject lightBallPrefab;

    private bool _inDanceMode = false;
    private float _beatTimer = 0f;
    private float _beatDuration => 60f / Mathf.Max(0.0001f, BPM);

    private Player _player;
    private readonly System.Collections.Generic.List<GameObject> _childrenDisabledByDance = new();
    private bool _playerDanceLightWasActive = false;

    private bool _advanceQueued = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Find global 2D light
        foreach (var l in FindObjectsOfType<Light2D>())
        {
            if (l.lightType == Light2D.LightType.Global)
            {
                _globalLight = l;
                _originalLightIntensity = l.intensity;
                break;
            }
        }

        if (ui) ui.ShowCanvas(false);
    }

    private void Update()
    {
        // Toggle dance mode (block if Fungus is speaking)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsFungusSpeaking()) return;

            if (!_inDanceMode) EnterDanceMode();
            else { ResetDanceMode(restartMusicNow: false); ExitDanceMode(); }
        }

        if (!_inDanceMode) return;

        // per-beat update
        _beatTimer += Time.deltaTime;
        bool crossedBeat = false;
        if (_beatTimer > _beatDuration)
        {
            crossedBeat = true;
            // If no input this beat -> miss
            if (!checker.InputReceivedThisBeat)
                HandleMiss();
            // next beat window
            _beatTimer -= _beatDuration;
            if (BeatEvent != null) BeatEvent.Post(gameObject);
            checker.StartNewBeatWindow();

            // If we queued a next-indicator after a successful hit, advance it only now (on beat)
            if (_advanceQueued)
            {
                _advanceQueued = false;
                ui.AdvanceToNextIndicatorNow();
            }
        }

        // grow current indicator
        if (ui) ui.UpdateIndicatorScale(_beatTimer, _beatDuration);

        // determine input acceptance window
        float timer = Mathf.Max(0f, _beatTimer);
        bool canAccept = timer <= inputBuffer || (_beatDuration - timer) <= inputBuffer;

        // consume click if any
        if (checker.TryConsumeInput(canAccept, _beatTimer, out bool inBuffer, out float tClick))
        {
            if (inBuffer)
            {
                // animate hit on current indicator
                int idx = ui.CurrentIndex;
                ui.OnHitGlowAndFinish(idx, tClick, _beatDuration);

                bool sequenceComplete = (ui.CurrentIndex + 1) >= ui.IndicatorCount;

                if (sequenceComplete)
                {
                    Color? fullComboColor = null;
                    if (checker.TryGetExactMatch(out var match) &&
                        ColorUtility.TryParseHtmlString(match.colorHex, out var c))
                    {
                        fullComboColor = c;
                    }

                    checker.currentRhythm.Add(match.colorName);
                    ui.CompleteSequence(fullComboColor);
                    checker.ResetAll();
                }
                else
                {
                    // Defer showing the next indicator until the next beat tick.
                    _advanceQueued = true;
                }

            }
            else
            {
                HandleMiss(); // outside buffer
            }
        }

        // test key to spawn light ball
        if (Input.GetKeyDown(KeyCode.L)) SpawnPlayerFadeLight(lightBallPrefab, Vector3.zero);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (checker.TryGetFinalRhythmResult(out var match))
            {
                Debug.Log(match.rhythmName);
            }
        }
    }

    private void EnterDanceMode()
    {
        _inDanceMode = true;

        // audio
        if (BackgroundMusic != null)
            _musicPlayingID_BG = BackgroundMusic.Post(gameObject);

        // lights
        if (_globalLight) _globalLight.intensity = danceLightIntensity;

        // UI
        if (ui)
        {
            ui.ShowCanvas(true);
            ui.ResetHardForNewGame();
        }

        // player
        DisablePlayer();

        // timers
        _beatTimer = -beatStartOffset;
        checker.StartNewBeatWindow();
    }

    private void ExitDanceMode()
    {
        _inDanceMode = false;

        // audio
        if (_musicPlayingID_BG != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID_BG);
            _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }

        // lights
        if (_globalLight) _globalLight.intensity = _originalLightIntensity;

        // UI
        if (ui) ui.ShowCanvas(false);

        // player
        RestorePlayer();
    }

    /// <summary>Hard reset the round (fresh UI rectangles, inputs, indicators, timers). Optionally restarts BG now.</summary>
    private void ResetDanceMode(bool restartMusicNow)
    {
        checker.ResetAll();
        if (ui) { ui.ResetHardForNewGame(); ui.SetOptionalUiVisible(true); }

        // timers
        _beatTimer = -beatStartOffset;
        checker.StartNewBeatWindow();

        // restart BG if requested
        if (restartMusicNow)
        {
            if (_musicPlayingID_BG != AkSoundEngine.AK_INVALID_PLAYING_ID)
            {
                AkSoundEngine.StopPlayingID(_musicPlayingID_BG);
                _musicPlayingID_BG = AkSoundEngine.AK_INVALID_PLAYING_ID;
            }
            if (BackgroundMusic != null)
                _musicPlayingID_BG = BackgroundMusic.Post(gameObject);
        }
    }

    private void HandleMiss()
    {
        // clear buffers (keep beat timer & music)
        checker.SoftReset();
        if (ui) ui.OnMissSnapBackToFirst();

        // camera shake (if present)
        var cam = Camera.main ? Camera.main.GetComponent<Chapter1_Camera>() : null;
        if (cam) cam.ShakeCamera();

        _advanceQueued = false;   // ensure no next indicator pops up
    }

    private bool IsFungusSpeaking()
    {
        return FindObjectsOfType<Flowchart>().Any(fc => fc.HasExecutingBlocks());
    }

    // ==== Player lock/unlock ====
    private void DisablePlayer()
    {
        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) _player = go.GetComponent<Player>();
        }
        if (!_player)
        {
            Debug.LogError("[DanceModeManager] Player not found.");
            return;
        }

        _player.canMove = false;
        _player.enabled = false;

        var rb = _player.GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = Vector2.zero;

        var anim = _player.GetComponent<Animator>();
        if (anim) anim.SetBool("isMoving", false);

        _childrenDisabledByDance.Clear();

        if (playerDanceLight) _playerDanceLightWasActive = playerDanceLight.activeSelf;

        foreach (Transform child in _player.transform)
        {
            var childGo = child.gameObject;
            if (playerDanceLight && childGo == playerDanceLight) continue;
            if (child.CompareTag("Lighting")) continue;

            if (childGo.activeSelf)
            {
                childGo.SetActive(false);
                _childrenDisabledByDance.Add(childGo);
            }
        }

        if (playerDanceLight) playerDanceLight.SetActive(true);
    }

    private void RestorePlayer()
    {
        if (!_player) return;

        _player.enabled = true;
        _player.canMove = true;

        foreach (var go in _childrenDisabledByDance)
            if (go) go.SetActive(true);
        _childrenDisabledByDance.Clear();

        if (playerDanceLight) playerDanceLight.SetActive(_playerDanceLightWasActive);
    }

    // ==== Optional light-ball helper ====
    public GameObject SpawnPlayerFadeLight(GameObject prefab, Vector3 localOffset)
    {
        var usePrefab = prefab;
        if (!usePrefab)
        {
            Debug.LogError("[DanceModeManager] LightBall prefab not assigned.");
            return null;
        }

        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) _player = go.GetComponent<Player>();
        }
        if (!_player)
        {
            Debug.LogError("[DanceModeManager] Player not found.");
            return null;
        }

        var inst = Instantiate(usePrefab, _player.transform);
        inst.transform.localPosition = localOffset;
        inst.transform.localRotation = Quaternion.identity;

        var effect = inst.GetComponent<DanceEffect_LightBall>();
        if (effect) effect.Begin();

        return inst;
    }
}
