using UnityEngine;

/// <summary>
/// Manages the current chapter of the game.
/// </summary>
public class ChapterManager : MonoBehaviour
{
    // Singleton instance
    public static ChapterManager Instance { get; private set; }

    // Enum representing different game chapters
    public enum Chapter
    {
        Chapter1,
        Chapter2
        // Add more chapters as needed
    }

    [SerializeField] private Chapter currentChapter = Chapter.Chapter1;

    /// <summary>
    /// Gets the current chapter.
    /// </summary>
    public Chapter CurrentChapter => currentChapter;

    private void Awake()
    {
        // Implement Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if necessary
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the current chapter to a new chapter.
    /// </summary>
    /// <param name="newChapter">The new chapter to set.</param>
    public void SetCurrentChapter(Chapter newChapter)
    {
        currentChapter = newChapter;
        Debug.Log($"Current Chapter set to: {currentChapter}");
    }

    /// <summary>
    /// Checks if the current chapter matches the specified chapter.
    /// </summary>
    /// <param name="chapter">The chapter to check against.</param>
    /// <returns>True if the current chapter is the specified chapter, otherwise false.</returns>
    public bool IsChapter(Chapter chapter)
    {
        return currentChapter == chapter;
    }
}
