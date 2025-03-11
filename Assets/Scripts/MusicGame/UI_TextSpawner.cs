using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TextSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private RectTransform spawnArea;   // The area where text is spawned.
    [SerializeField] private GameObject textPrefab;       // Prefab for the UI text.
    
    [Header("Time Interval Settings")]
    [SerializeField] private bool randomIntervals = true; // If true, use random intervals.
    [SerializeField] private float minSpawnTime = 0.5f;     // Minimum wait time (random mode).
    [SerializeField] private float maxSpawnTime = 2f;       // Maximum wait time (random mode).
    // Fixed wait time (if series mode and no series provided).
    [SerializeField] private float[] intervalSeries;        // Array of intervals for series mode.
    private float lastWaitTime = -1f;                       // To store the last chosen interval.

    [Header("Text Settings")]
    [SerializeField] private bool randomTexts = true;       // If true, choose messages randomly.
    [SerializeField] private string[] messages;             // Array of messages.
    private int messageSeriesIndex = 0;

    // Duration for the fade in/out effect (total duration is divided among fade in, hold, fade out).
    [SerializeField] private float totalFadeDuration = 3f; 

    private void Start()
    {
        StartCoroutine(SpawnTextRoutine());
    }

    private IEnumerator SpawnTextRoutine()
    {
        while (true)
        {
            float waitTime = GetNextWaitTime();
            yield return new WaitForSeconds(waitTime);
            SpawnText();
        }
    }

    /// <summary>
    /// Determines the wait time before the next spawn.
    /// If randomIntervals is false (set intervals mode), this selects a random interval from the series array
    /// that is different from the last one (if possible).
    /// </summary>
    private float GetNextWaitTime()
    {
        if (randomIntervals)
        {
            return Random.Range(minSpawnTime, maxSpawnTime);
        }
        if (intervalSeries != null && intervalSeries.Length > 0)
        {
            // If there's only one interval, there's no choice.
            if (intervalSeries.Length == 1)
            {
                lastWaitTime = intervalSeries[0];
                return lastWaitTime;
            }

            // Choose a random interval from the series that is different from the last chosen interval.
            float waitTime = intervalSeries[Random.Range(0, intervalSeries.Length)];
            while (waitTime == lastWaitTime && intervalSeries.Length > 1)
            {
                waitTime = intervalSeries[Random.Range(0, intervalSeries.Length)];
            }
            lastWaitTime = waitTime;
            return waitTime;
        }
    
        Debug.LogWarning("No interval series provided; returning 1 as wait time.");
        return 1f;
    }

    private void SpawnText()
    {
        GameObject newTextObject = Instantiate(textPrefab, spawnArea);
        
        Vector2 randomPos = new Vector2(
            Random.Range(-spawnArea.rect.width / 2f, spawnArea.rect.width / 2f),
            Random.Range(-spawnArea.rect.height / 2f, spawnArea.rect.height / 2f)
        );
        
        RectTransform rt = newTextObject.GetComponent<RectTransform>();
        rt.anchoredPosition = randomPos;
        
        if (messages != null && messages.Length > 0)
        {
            string message = GetNextMessage();
            var tmpText = newTextObject.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = message;
            }
            else
            {
                // fallback for UnityEngine.UI.Text if needed
                Text uiText = newTextObject.GetComponent<Text>();
                if (uiText != null)
                {
                    uiText.text = message;
                }
            }
        }
        
        // Instead of immediate destruction, start the fade in/out effect.
        StartCoroutine(FadeInAndOut(newTextObject, totalFadeDuration));
    }

    private string GetNextMessage()
    {
        if (messages == null || messages.Length == 0)
            return "";

        if (randomTexts)
        {
            return messages[Random.Range(0, messages.Length)];
        }
        else
        {
            string msg = messages[messageSeriesIndex];
            messageSeriesIndex = (messageSeriesIndex + 1) % messages.Length;
            return msg;
        }
    }

    /// <summary>
    /// Gradually fades the text in, holds it, then fades it out before destroying the object.
    /// </summary>
    /// <param name="textObject">The spawned text GameObject.</param>
    /// <param name="totalDuration">Total duration of the fade effect.</param>
    private IEnumerator FadeInAndOut(GameObject textObject, float totalDuration)
    {
        // Define durations for fade in and fade out. The hold duration is totalDuration minus fade durations.
        float fadeInTime = 0.5f;
        float fadeOutTime = 0.5f;
        float holdTime = totalDuration - fadeInTime - fadeOutTime;
        if (holdTime < 0) holdTime = 0f;

        // Get the text component (TMP or UI Text) and store its original color.
        TMPro.TextMeshProUGUI tmpText = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        Text uiText = null;
        Color originalColor = Color.white;
        if (tmpText != null)
        {
            originalColor = tmpText.color;
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
        else
        {
            uiText = textObject.GetComponent<Text>();
            if (uiText != null)
            {
                originalColor = uiText.color;
                uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            }
            else
            {
                Debug.LogWarning("No text component found on spawned object.");
                yield break;
            }
        }

        // Fade In: alpha from 0 to 1.
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeInTime);
            if (tmpText != null)
            {
                tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            else if (uiText != null)
            {
                uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            yield return null;
        }

        // Hold the text at full opacity.
        yield return new WaitForSeconds(holdTime);

        // Fade Out: alpha from 1 to 0.
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (timer / fadeOutTime));
            if (tmpText != null)
            {
                tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            else if (uiText != null)
            {
                uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            yield return null;
        }

        // Destroy the text object once the fade out is complete.
        Destroy(textObject);
    }
}
