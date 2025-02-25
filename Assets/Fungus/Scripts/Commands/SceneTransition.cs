using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  // Required for scene management

public class SpriteZoom : MonoBehaviour
{
    [SerializeField] private GameObject spriteToZoom; // The sprite GameObject to affect
    [SerializeField] private float zoomSpeed = 0.3f;      // Controls how fast the zoom and fade in occur
    [SerializeField] private float zoomAmount = 1.2f;     // The target scale multiplier (e.g., 1.2 means 120% of original)
    [SerializeField] private string sceneName;            // The name of the scene to load after fade out

    private void Start()
    {
        if (spriteToZoom != null)
        {
            StartCoroutine(ZoomAndFadeCoroutine());
        }
        else
        {
            Debug.LogError("Sprite to zoom is not assigned!");
        }
    }

    private IEnumerator ZoomAndFadeCoroutine()
    {
        // Cache the initial and target scales
        Vector3 initialScale = spriteToZoom.transform.localScale;
        Vector3 targetScale = initialScale * zoomAmount;

        SpriteRenderer sr = spriteToZoom.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on the spriteToZoom GameObject.");
            yield break;
        }

        // Set the sprite's alpha to 0
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;

        // Fade in and zoom concurrently
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;
            // Lerp scale from initial to target
            spriteToZoom.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            // Lerp alpha
            color.a = Mathf.Lerp(0f, 1f, t);
            sr.color = color;
            yield return null;
        }

        // Ensure final scale and opacity
        spriteToZoom.transform.localScale = targetScale;
        color.a = 1f;
        sr.color = color;

        // Fade out quickly
        float fadeT = 0f;
        float fadeOutSpeed = zoomSpeed * 2f;
        while (fadeT < 1f)
        {
            fadeT += Time.deltaTime * fadeOutSpeed;
            // Lerp alpha from 1 to 0.
            color.a = Mathf.Lerp(1f, 0f, fadeT);
            sr.color = color;
            yield return null;
        }

        color.a = 0f;
        sr.color = color;

        SceneManager.LoadScene(sceneName);
    }
}
