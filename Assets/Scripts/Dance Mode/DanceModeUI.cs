using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceModeUI : MonoBehaviour
{
    [Header("Canvas Root")]
    [SerializeField] private Canvas danceModeCanvas;

    [Header("Indicators")]
    [Tooltip("Order matters – one per beat.")]
    [SerializeField] private List<RectTransform> beatIndicators = new();
    [SerializeField] private float scaleSpeed = 1.2f;
    [SerializeField] private float maxScaleFactor = 40f;
    private readonly List<Vector3> _initialScales = new();

    [Header("Hit Feedback")]
    [SerializeField] private Sprite indicatorGlowSprite;
    [SerializeField] private float correctGlowSeconds = 0.25f;
    [SerializeField] private float indicatorFadeSeconds = 0.35f;

    [Header("Result Display")]
    [SerializeField] private Image resultImage; // tints on full combo (else black)
    [SerializeField] private Image rectanglePrefab;
    [SerializeField] private RectTransform rectanglesParent;
    [SerializeField] private float rectangleSpacing = 50f;
    [SerializeField] private float rectangleStartX = -350f;
    [SerializeField] private float rectangleStartY = 0f;

    [Header("Optional UI")]
    [SerializeField] private GameObject uiDisappearObject;

    private readonly List<Image> _spawnedRectangles = new();
    private readonly Dictionary<Image, Coroutine> _runningFades = new();
    private int _currentIndicatorIndex = 0;

    public int IndicatorCount => beatIndicators.Count;
    public int CurrentIndex => _currentIndicatorIndex;

    private void Awake()
    {
        _initialScales.Clear();
        foreach (var rt in beatIndicators)
        {
            _initialScales.Add(rt.localScale);
            rt.gameObject.SetActive(false);
        }
        if (danceModeCanvas != null) danceModeCanvas.gameObject.SetActive(false);
        if (resultImage != null) resultImage.color = Color.black;
    }

    public void ShowCanvas(bool on)
    {
        if (danceModeCanvas) danceModeCanvas.gameObject.SetActive(on);
    }

    public void ResetHardForNewGame()
    {
        // result color
        if (resultImage) resultImage.color = Color.black;

        // remove rectangles
        for (int i = _spawnedRectangles.Count - 1; i >= 0; i--)
        {
            if (_spawnedRectangles[i]) Destroy(_spawnedRectangles[i].gameObject);
        }
        _spawnedRectangles.Clear();

        // restore optional UI object
        if (uiDisappearObject) uiDisappearObject.SetActive(true);

        // indicators
        HideAllIndicators();
        for (int i = 0; i < beatIndicators.Count; i++)
            beatIndicators[i].localScale = _initialScales[i];

        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
    }

    public void SoftResetFeedbackOnly()
    {
        if (resultImage) resultImage.color = Color.black;
    }

    public void HideAllIndicators()
    {
        foreach (var rt in beatIndicators) rt.gameObject.SetActive(false);
    }

    public void RestartIndicatorsToFirstBeat()
    {
        HideAllIndicators();
        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
    }

    public void ShowCurrentIndicator()
    {
        var rt = beatIndicators[_currentIndicatorIndex];
        rt.gameObject.SetActive(true);
        var img = rt.GetComponent<Image>();
        if (img)
        {
            var c = img.color; c.a = 1f; img.color = c;
        }
        rt.localScale = Vector3.zero;
    }

    public void UpdateIndicatorScale(float beatTimer, float beatDuration)
    {
        var rt = beatIndicators[_currentIndicatorIndex];
        float timer = Mathf.Max(0f, beatTimer);
        float t = Mathf.Clamp01((timer * scaleSpeed) / Mathf.Max(0.0001f, beatDuration));
        float s = Mathf.Lerp(0f, maxScaleFactor, t);
        rt.localScale = _initialScales[_currentIndicatorIndex] * s;
    }

    public void OnHitGlowAndFinish(int index, float startBeatTimer, float beatDuration)
    {
        FlashIndicatorGlow(index);
        StartIndicatorFinishScaleAndFade(index, startBeatTimer, beatDuration, correctGlowSeconds, indicatorFadeSeconds);
    }

    public void AdvanceIndicatorOrComplete(bool sequenceComplete, Color? fullComboColorIfAny)
    {
        int justHit = _currentIndicatorIndex;

        if (sequenceComplete)
        {
            // Apply color & spawn rectangle
            if (fullComboColorIfAny.HasValue)
            {
                var c = fullComboColorIfAny.Value;
                if (resultImage) resultImage.color = c;

                if (rectanglePrefab && rectanglesParent)
                {
                    var img = Instantiate(rectanglePrefab, rectanglesParent);
                    img.color = c;
                    var rt = img.rectTransform;
                    rt.anchoredPosition = new Vector2(rectangleStartX + _spawnedRectangles.Count * rectangleSpacing,
                                                      rectangleStartY);
                    _spawnedRectangles.Add(img);
                }
            }
            else
            {
                if (resultImage) resultImage.color = Color.black;
            }

            // reset indicators for next round
            HideAllIndicatorsExcept(justHit);
            _currentIndicatorIndex = 0;
            ShowCurrentIndicator();
            return;
        }

        // mid-combo: move to next indicator
        _currentIndicatorIndex++;
        if (_currentIndicatorIndex < beatIndicators.Count)
        {
            ShowCurrentIndicator();
        }
    }

    public void HideAllIndicatorsExcept(int keepIdx)
    {
        for (int i = 0; i < beatIndicators.Count; i++)
        {
            if (i == keepIdx) continue;
            beatIndicators[i].gameObject.SetActive(false);
        }
    }

    public void OnMissSnapBackToFirst()
    {
        SoftResetFeedbackOnly();
        RestartIndicatorsToFirstBeat();
    }

    public void SetOptionalUiVisible(bool on)
    {
        if (uiDisappearObject) uiDisappearObject.SetActive(on);
    }

    // === visuals helpers ===
    private void FlashIndicatorGlow(int index)
    {
        if (index < 0 || index >= beatIndicators.Count) return;
        var srcRT = beatIndicators[index];
        var srcImg = srcRT.GetComponent<Image>();
        if (!srcImg || !indicatorGlowSprite) return;

        var glowGO = new GameObject("IndicatorGlowTemp");
        var parent = srcRT.parent as RectTransform;
        var glowRT = glowGO.AddComponent<RectTransform>();
        glowRT.SetParent(parent, worldPositionStays: false);
        glowRT.SetAsLastSibling();
        glowRT.anchorMin = srcRT.anchorMin;
        glowRT.anchorMax = srcRT.anchorMax;
        glowRT.pivot = srcRT.pivot;
        glowRT.anchoredPosition = srcRT.anchoredPosition;
        glowRT.sizeDelta = srcRT.sizeDelta;
        glowRT.localRotation = srcRT.localRotation;
        glowRT.localScale = srcRT.localScale;

        var glowImg = glowGO.AddComponent<Image>();
        glowImg.sprite = indicatorGlowSprite;
        glowImg.raycastTarget = false;
        glowImg.color = Color.white;

        StartCoroutine(FadeAndDestroy(glowImg, correctGlowSeconds));
    }

    private IEnumerator FadeAndDestroy(Graphic g, float dur)
    {
        float t = 0f;
        var start = g.color;
        while (t < dur && g)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(1f - t / Mathf.Max(0.0001f, dur));
            g.color = new Color(start.r, start.g, start.b, a);
            yield return null;
        }
        if (g) Destroy(g.gameObject);
    }

    private void StartIndicatorFinishScaleAndFade(int index, float startBeatTimer, float beatDuration,
                                                  float postMaxHoldSec, float fadeSec)
    {
        if (index < 0 || index >= beatIndicators.Count) return;

        var img = beatIndicators[index].GetComponent<Image>();
        if (!img) return;

        if (_runningFades.TryGetValue(img, out var prev) && prev != null)
        {
            StopCoroutine(prev);
            var c = img.color; c.a = 1f; img.color = c;
        }

        var co = StartCoroutine(FinishExpandThenFade(index, startBeatTimer, beatDuration, postMaxHoldSec, fadeSec));
        _runningFades[img] = co;
    }

    private IEnumerator FinishExpandThenFade(int index, float startBeatTimer, float beatDuration,
                                             float postMaxHoldSec, float fadeSec)
    {
        var rt = beatIndicators[index];
        var img = rt.GetComponent<Image>();
        if (!img) yield break;

        float timer = Mathf.Max(0f, startBeatTimer);
        float targetTimerForMax = beatDuration / Mathf.Max(0.0001f, 1f); // scaleSpeed already applied in UpdateIndicatorScale

        // Expand to max
        while (rt && img && timer < targetTimerForMax)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01((timer) / Mathf.Max(0.0001f, targetTimerForMax));
            float s = Mathf.Lerp(0f, maxScaleFactor, t);
            rt.localScale = _initialScales[index] * s;
            yield return null;
        }

        // Optional hold
        float hold = 0f;
        while (img && hold < postMaxHoldSec)
        {
            hold += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out
        var startColor = img.color;
        float ft = 0f;
        while (img && ft < fadeSec)
        {
            ft += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startColor.a, 0f, ft / Mathf.Max(0.0001f, fadeSec));
            var c = img.color; c.a = a; img.color = c;
            yield return null;
        }

        if (img)
        {
            var c = img.color; c.a = startColor.a; img.color = c;
            img.gameObject.SetActive(false);
            _runningFades.Remove(img);
        }
    }

    // Call when a full combo is completed (tint + rectangle + reset to first)
    public void CompleteSequence(Color? fullComboColorIfAny)
    {
        if (fullComboColorIfAny.HasValue && resultImage)
        {
            resultImage.color = fullComboColorIfAny.Value;

            if (rectanglePrefab && rectanglesParent)
            {
                var img = Instantiate(rectanglePrefab, rectanglesParent);
                img.color = fullComboColorIfAny.Value;
                var rt = img.rectTransform;
                rt.anchoredPosition = new Vector2(rectangleStartX + _spawnedRectangles.Count * rectangleSpacing,
                                                  rectangleStartY);
                _spawnedRectangles.Add(img);
            }
        }
        else
        {
            if (resultImage) resultImage.color = Color.black;
        }

        // reset indicators for next round, stay on first
        HideAllIndicatorsExcept(_currentIndicatorIndex);
        _currentIndicatorIndex = 0;
        ShowCurrentIndicator();
    }

    // Advance to the next indicator *now* (used on the next beat tick)
    public void AdvanceToNextIndicatorNow()
    {
        _currentIndicatorIndex++;
        if (_currentIndicatorIndex < beatIndicators.Count)
        {
            ShowCurrentIndicator();
        }
        else
        {
            // Safety: if somehow beyond last, clamp back to last valid or reset
            _currentIndicatorIndex = Mathf.Clamp(_currentIndicatorIndex, 0, Mathf.Max(0, beatIndicators.Count - 1));
        }
    }

}
