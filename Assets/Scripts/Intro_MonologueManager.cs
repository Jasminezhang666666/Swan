using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class MonologueManager : MonoBehaviour
{
    // Serialized fields for spacing and transparency behavior
    [SerializeField] private float lineSpacing = 10f;           // Space between lines
    [SerializeField] private float spaceBeforeFirstLine = 20f;  // Space before the first line
    [SerializeField] private float fadeInDuration = 1f;         // Time it takes for each line to fully appear
    [SerializeField] private float delayBetweenLines = 0.5f;    // Delay between each line's appearance
    [SerializeField] private VideoPlayer videoPlayer;           // Reference to the existing VideoPlayer in the scene
    private CanvasGroup canvasGroup;                           // Reference to the CanvasGroup

    void Start()
    {
        // Get the CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup is not attached to the Canvas.");
        }

        AdjustLineSpacing();          // Set the spacing
        StartCoroutine(FadeInText()); // Start the fade-in effect
    }

    // Adjust the spacing between lines
    void AdjustLineSpacing()
    {
        // Get all the TextMeshProUGUI children of this GameObject
        TextMeshProUGUI[] textLines = GetComponentsInChildren<TextMeshProUGUI>();

        if (textLines.Length == 0) return; // Safety check

        float currentYPosition = -spaceBeforeFirstLine;

        // Iterate through each line and set their initial position
        for (int i = 0; i < textLines.Length; i++)
        {
            RectTransform rectTransform = textLines[i].rectTransform;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, currentYPosition, rectTransform.localPosition.z);

            // Start each line as fully transparent
            SetAlpha(textLines[i], 0f);

            // Calculate the next Y position
            currentYPosition -= rectTransform.rect.height + lineSpacing;
        }
    }

    // Coroutine to gradually fade in each text line one by one
    IEnumerator FadeInText()
    {
        // Get all the TextMeshProUGUI children
        TextMeshProUGUI[] textLines = GetComponentsInChildren<TextMeshProUGUI>();

        // Fade in each line one by one
        foreach (TextMeshProUGUI textLine in textLines)
        {
            // Start fading the line in
            yield return StartCoroutine(FadeInLine(textLine));

            // Wait before showing the next line
            yield return new WaitForSeconds(delayBetweenLines);
        }

        // After all the text has faded in, hide the UI and play the video
        HideCanvasAndPlayVideo();
    }

    // Coroutine to fade in a single line
    IEnumerator FadeInLine(TextMeshProUGUI textLine)
    {
        float elapsedTime = 0f;
        Color originalColor = textLine.color;

        // Gradually increase alpha from 0 to 1
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration); // Calculate alpha between 0 and 1
            SetAlpha(textLine, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 1 (fully visible)
        SetAlpha(textLine, 1f);
    }

    // Method to hide the Canvas and its children
    void HideCanvasAndPlayVideo()
    {
        // Start playing the video
        videoPlayer.Play();

        // Start a coroutine to wait for 0.8 seconds before hiding the canvas
        StartCoroutine(HideCanvasAfterDelay(0.8f));
    }

    // Coroutine to wait for a specified delay before hiding the Canvas
    IEnumerator HideCanvasAfterDelay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Hide the canvas by adjusting the CanvasGroup properties
        if (canvasGroup != null)
        {
            // Set the alpha of the CanvasGroup to 0 to make the entire Canvas and its children invisible
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false; // Make it non-interactable
            canvasGroup.blocksRaycasts = false; // Disable blocking raycasts
        }
    }


    // Helper function to set the alpha (transparency) of a TextMeshProUGUI
    void SetAlpha(TextMeshProUGUI textLine, float alpha)
    {
        Color color = textLine.color;
        color.a = alpha; // Modify only the alpha value
        textLine.color = color;
    }

    // Allows real-time adjustments of the line spacing in the editor
    void OnValidate()
    {
        AdjustLineSpacing();
    }
}
