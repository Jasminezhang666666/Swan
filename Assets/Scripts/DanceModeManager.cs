using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using AK;
using AK.Wwise;
using AKEvent = AK.Wwise.Event;

[RequireComponent(typeof(DanceMode))]
public class DanceModeManager : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float danceLightIntensity = 0.05f;
    private float _originalLightIntensity;
    private Light2D _globalLight;

    [Header("UI Indicator")]
    [Tooltip("UI element (RectTransform) that shrinks each beat")]
    [SerializeField] private RectTransform beatIndicator;
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

    private DanceMode _danceMode;
    private bool _inDanceMode;
    private float _beatTimer;
    private bool _canAcceptInput;
    private Vector3 _initialIndicatorScale;
    private Player _player;
    private GameObject _playerLightingChild;

    private void Awake()
    {
        _danceMode = GetComponent<DanceMode>();
    }

    private void Start()
    {
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
        _danceMode.enabled = false;

        if (beatIndicator != null)
        {
            beatIndicator.gameObject.SetActive(false);
            _initialIndicatorScale = beatIndicator.localScale;
        }

        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_inDanceMode) EnterDanceMode();
            else ExitDanceMode();
        }

        if (_inDanceMode)
        {
            UpdateBeatIndicator();
            HandleBeatInput();
        }
    }

    private void EnterDanceMode()
    {
        _inDanceMode = true;

        // disable player movement/animation
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
        else Debug.LogError("DanceModeManager: No Player tagged 'Player' found.");

        // dim the global light
        if (_globalLight != null)
            _globalLight.intensity = danceLightIntensity;

        // enable the dance-mode logic
        _danceMode.enabled = true;

        // show beat circle
        if (beatIndicator != null)
        {
            beatIndicator.gameObject.SetActive(true);
            beatIndicator.localScale = _initialIndicatorScale;
        }

        // show full dance UI
        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(true);

        _beatTimer = -beatStartOffset;
        _musicPlayingID = Snd_44.Post(gameObject);
    }

    private void ExitDanceMode()
    {
        _inDanceMode = false;

        // restore player
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

        // restore lighting
        if (_globalLight != null)
            _globalLight.intensity = _originalLightIntensity;

        // disable dance logic
        _danceMode.enabled = false;

        // hide beat circle
        if (beatIndicator != null)
            beatIndicator.gameObject.SetActive(false);

        // hide full dance UI
        if (danceModeCanvas != null)
            danceModeCanvas.gameObject.SetActive(false);

        // stop music
        if (_musicPlayingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID);
            _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
    }

    private void UpdateBeatIndicator()
    {
        _beatTimer += Time.deltaTime;
        if (_beatTimer > beatDuration) _beatTimer -= beatDuration;

        float timer = _beatTimer < 0f ? 0f : _beatTimer;
        float rawT = (timer * shrinkSpeed) / beatDuration;
        float t = Mathf.Clamp01(rawT);

        float scaleFactor = Mathf.Lerp(1f, minScaleFactor, t);
        if (beatIndicator != null)
            beatIndicator.localScale = _initialIndicatorScale * scaleFactor;

        _canAcceptInput = (timer <= inputBuffer) || (beatDuration - timer <= inputBuffer);
    }

    private void HandleBeatInput()
    {
        if (!_danceMode.enabled) return;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (_canAcceptInput)
            {
                // valid input
            }
            else Debug.Log("Missed beat!");
        }
    }
}
