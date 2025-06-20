using UnityEngine;
using UnityEngine.Rendering.Universal;  // for Light2D

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

    private void Awake()
    {
        // cache reference to your DanceMode script
        _danceMode = GetComponent<DanceMode>();
    }

    private void Start()
    {
        // find and cache the global 2D light
        foreach (var l in FindObjectsOfType<Light2D>())
        {
            if (l.lightType == Light2D.LightType.Global)
            {
                _globalLight = l;
                _originalLightIntensity = l.intensity;
                break;
            }
        }
        if (_globalLight == null)
            Debug.LogError("DanceModeManager: No Light2D of type Global found in scene.");

        // ensure dance mode is off until triggered
        _danceMode.enabled = false;
    }

    private void Update()
    {
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

        // 1) Find Player by tag
        if (_player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) _player = go.GetComponent<Player>();
        }

        if (_player != null)
        {
            // stop player movement & disable player script
            _player.canMove = false;
            _player.enabled = false;

            // zero out velocity
            var rb = _player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            // force idle animation & sprite
            var anim = _player.GetComponent<Animator>();
            if (anim != null) anim.SetBool("isMoving", false);
            var sr = _player.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            // deactivate all non-lighting children
            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting"))
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    // cache and activate the lighting child
                    _playerLightingChild = child.gameObject;
                    _playerLightingChild.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogError("DanceModeManager: No GameObject tagged 'Player' with a Player component found.");
        }

        // 2) Dim global light
        if (_globalLight != null)
        {
            _globalLight.intensity = danceLightIntensity;
        }

        // 3) Enable rhythm-input script
        _danceMode.enabled = true;
    }

    private void ExitDanceMode()
    {
        _inDanceMode = false;

        // restore player movement & enable player script
        if (_player != null)
        {
            _player.enabled = true;
            _player.canMove = true;

            // restore non-lighting children
            foreach (Transform child in _player.transform)
            {
                if (!child.CompareTag("Lighting"))
                    child.gameObject.SetActive(true);
                else if (_playerLightingChild != null)
                    _playerLightingChild.SetActive(false);
            }
        }

        // restore light
        if (_globalLight != null)
            _globalLight.intensity = _originalLightIntensity;

        // disable rhythm mode
        _danceMode.enabled = false;
    }
}
