using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
[RequireComponent(typeof(Light2D))]
public class DanceEffect_LightBall : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("How long it takes for the light to disappear completely (seconds).")]
    [SerializeField] private float disappearDuration = 5f;

    [Tooltip("Automatically start shrinking on Start(). If false, call Begin() manually.")]
    [SerializeField] private bool autoStart = true;

    [Tooltip("Also fade intensity to 0 while shrinking.")]
    [SerializeField] private bool fadeIntensity = true;

    private Light2D _light;
    private float _elapsed;
    private bool _running;

    private bool _isPoint;
    private float _startOuterRadius;   // Point only
    private float _startInnerRadius;   // Point only

    private Vector3 _startScale;       // Non-Point fallback
    private float _startIntensity;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _isPoint = (_light.lightType == Light2D.LightType.Point);

        _startScale = transform.localScale;
        if (_startScale == Vector3.zero) _startScale = Vector3.one;

        _startIntensity = _light.intensity;

        if (_isPoint)
        {
            _startOuterRadius = _light.pointLightOuterRadius;
            _startInnerRadius = _light.pointLightInnerRadius;

            // Safety: ensure inner <= outer at start
            if (_startInnerRadius > _startOuterRadius)
                _startInnerRadius = _startOuterRadius;
        }
    }

    private void Start()
    {
        if (autoStart) Begin();
    }

    /// <summary>Starts shrinking the light over its configured duration.</summary>
    public void Begin()
    {
        if (disappearDuration <= 0f)
        {
            ApplyProgress(1f);
            Destroy(gameObject);
            return;
        }
        _elapsed = 0f;
        _running = true;
    }

    private void Update()
    {
        if (!_running) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / Mathf.Max(0.0001f, disappearDuration));
        ApplyProgress(t);

        if (t >= 1f)
        {
            _running = false;
            Destroy(gameObject);
        }


    }

    /// <summary>t: 0 -> start, 1 -> fully gone</summary>
    private void ApplyProgress(float t)
    {
        // Ease-out for a nicer feel
        float ease = 1f - Mathf.Pow(1f - t, 2f);

        if (_isPoint)
        {
            // Shrink both radii together so inner stays <= outer
            float outer = Mathf.Lerp(_startOuterRadius, 0f, ease);
            float inner = Mathf.Lerp(_startInnerRadius, 0f, ease);
            inner = Mathf.Min(inner, outer); // guarantee inner <= outer

            _light.pointLightOuterRadius = outer;
            _light.pointLightInnerRadius = inner;
        }
        else
        {
            // Freeform/Sprite/Parametric/Global: no inner radius concept, so scale down
            transform.localScale = Vector3.Lerp(_startScale, Vector3.zero, ease);
        }

        if (fadeIntensity)
        {
            _light.intensity = Mathf.Lerp(_startIntensity, 0f, ease);
        }
    }
}
