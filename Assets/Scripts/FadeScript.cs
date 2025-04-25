using UnityEngine;
using System.Collections;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public float fadeDuration = 5.0f;

    [Tooltip("If true, will automatically play FadeIn() in Start()")]
    [SerializeField] private bool fadeInOnStart = false;

    private void Start()
    {
        if (fadeInOnStart)
        {
            FadeIn();
        }
    }

    /// <summary>
    /// Fade from alpha=1 to alpha=0 over fadeDuration.
    /// </summary>
    public void FadeIn()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1f;
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
    }

    /// <summary>
    /// Fade from alpha=0 to alpha=1 over fadeDuration.
    /// </summary>
    public void FadeOut()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}
