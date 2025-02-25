using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutScene_Tripped : MonoBehaviour
{
    [Header("Assign your three sprite renderers here")]
    [SerializeField] private SpriteRenderer[] sprites; // Drag three game objects with SpriteRenderer components

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f; // Duration for fading in/out
    [SerializeField] private float waitTime = 2f;     // Wait time after all images are fully visible

    [Header("Scene Transition")]
    [SerializeField] private string sceneTransition;  // Name of the scene to load after the cutscene

    private void Start()
    {
        // Ensure every sprite starts with alpha 0
        foreach (var sprite in sprites)
        {
            if (sprite != null)
            {
                Color c = sprite.color;
                c.a = 0f;
                sprite.color = c;
            }
        }

        // Start the cutscene sequence
        StartCoroutine(RunCutScene());
    }

    private IEnumerator RunCutScene()
    {
        // Fade in each sprite one after the other
        foreach (var sprite in sprites)
        {
            yield return StartCoroutine(Fade(sprite, 0f, 1f, fadeDuration));
        }

        yield return new WaitForSeconds(waitTime);

        // Fade out all sprites simultaneously
        yield return StartCoroutine(FadeAll(sprites, 1f, 0f, fadeDuration));

        // Transition to another scene
        if (!string.IsNullOrEmpty(sceneTransition))
        {
            SceneManager.LoadScene(sceneTransition);
        }
    }

    private IEnumerator Fade(SpriteRenderer sprite, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            Color c = sprite.color;
            c.a = currentAlpha;
            sprite.color = c;
            yield return null;
        }
        // Ensure the final alpha value is set
        Color finalColor = sprite.color;
        finalColor.a = endAlpha;
        sprite.color = finalColor;
    }

    private IEnumerator FadeAll(SpriteRenderer[] sprites, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    Color c = sprite.color;
                    c.a = currentAlpha;
                    sprite.color = c;
                }
            }
            yield return null;
        }
        foreach (var sprite in sprites)
        {
            if (sprite != null)
            {
                Color c = sprite.color;
                c.a = endAlpha;
                sprite.color = c;
            }
        }
    }
}
