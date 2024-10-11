using System.Collections;
using UnityEngine;
using TMPro;

public class MonologueManager : MonoBehaviour
{
    // Serialized fields for spacing and transparency behavior
    [SerializeField] private float lineSpacing = 10f;           // Space between lines
    [SerializeField] private float spaceBeforeFirstLine = 20f;  // Space before the first line
    [SerializeField] private float fadeInDuration = 1f;         // Time it takes for each line to fully appear
    [SerializeField] private float delayBetweenLines = 0.5f;    // Delay between each line's appearance

    void Start()
    {
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
    }

    // Coroutine to fade in a single line
    IEnumerator FadeInLine(TextMeshProUGUI textLine)
    {
        float elapsedTime = 0f;
        Color originalColor = textLine.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

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
