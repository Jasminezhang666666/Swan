using UnityEngine;
using UnityEngine.Rendering.Universal;  // For Light2D
using AK;                                // For AkSoundEngine & AK_INVALID_PLAYING_ID
using AKEvent = AK.Wwise.Event;         // Alias to avoid ambiguity with UnityEngine.Event

[RequireComponent(typeof(DanceMode))]
public class DanceModeManager : MonoBehaviour
{
    [Header("Light Settings")]
    [Tooltip("Intensity to set on the Global 2D Light when dance mode starts")]
    [SerializeField] private float danceLightIntensity = 0.05f;

    private float _originalLightIntensity;
    private Light2D _globalLight;
    private Player _player;
    private DanceMode _danceMode;
    private bool _inDanceMode = false;
    private GameObject _playerLightingChild;

    [Header("Wwise Music Event")]
    [Tooltip("Assign your '44' music event here")]
    public AKEvent Snd_44;
    private uint _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;

    private void Awake()
    {
        // Cache reference to the DanceMode script
        _danceMode = GetComponent<DanceMode>();
    }

    private void Start()
    {
        // Find and cache the global 2D light
        foreach (var light in FindObjectsOfType<Light2D>())
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                _globalLight = light;
                _originalLightIntensity = light.intensity;
                break;
            }
        }
        if (_globalLight == null)
            Debug.LogError("DanceModeManager: No Light2D of type Global found in scene.");

        // Ensure dance mode logic is off until triggered
        _danceMode.enabled = false;
    }

    private void Update()
    {
        // Toggle dance mode with the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_inDanceMode)
                EnterDanceMode();
            else
                ExitDanceMode();
        }
    }

    private void EnterDanceMode()
    {
        _inDanceMode = true;

        // 1) Find and disable the Player
        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                _player = go.GetComponent<Player>();
        }

        if (_player != null)
        {
            _player.canMove = false;
            _player.enabled = false;

            // Zero out velocity
            var rb = _player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            // Force idle animation/sprite
            var anim = _player.GetComponent<Animator>();
            if (anim != null) anim.SetBool("isMoving", false);
            var sr = _player.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            // Hide all children except the one tagged "Lighting"
            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting"))
                    child.gameObject.SetActive(false);
                else
                {
                    _playerLightingChild = child.gameObject;
                    _playerLightingChild.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogError("DanceModeManager: No GameObject tagged 'Player' found.");
        }

        // 2) Dim the global light
        if (_globalLight != null)
            _globalLight.intensity = danceLightIntensity;

        // 3) Enable the rhythm-input script
        _danceMode.enabled = true;

        // 4) Play the Wwise music and capture its playing ID
        _musicPlayingID = Snd_44.Post(gameObject);
    }

    private void ExitDanceMode()
    {
        _inDanceMode = false;

        // 1) Restore the Player
        if (_player != null)
        {
            _player.enabled = true;
            _player.canMove = true;

            // Restore all non-lighting children
            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting"))
                    child.gameObject.SetActive(true);
                else if (_playerLightingChild != null)
                    _playerLightingChild.SetActive(false);
            }
        }

        // 2) Restore the global light intensity
        if (_globalLight != null)
            _globalLight.intensity = _originalLightIntensity;

        // 3) Disable the rhythm-input script
        _danceMode.enabled = false;

        // 4) Stop the Wwise music
        StopMusic();
    }

    /// <summary>
    /// Stops the currently playing Wwise music event.
    /// </summary>
    public void StopMusic()
    {
        if (_musicPlayingID != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            AkSoundEngine.StopPlayingID(_musicPlayingID);
            _musicPlayingID = AkSoundEngine.AK_INVALID_PLAYING_ID;
        }
    }
}
