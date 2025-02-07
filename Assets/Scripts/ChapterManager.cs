using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    // Singleton instance
    public static ChapterManager Instance { get; private set; }

    // Enum representing different game chapters
    public enum Chapter
    {
        Chapter1,
        Chapter1_3,
        Chapter2
        // Add more chapters as needed
    }

    [SerializeField] private Chapter currentChapter = Chapter.Chapter1;
    public Chapter CurrentChapter => currentChapter;

    public bool Chp1_LookedAtStage = false;
    public bool Chp1_PlayedPuzzle1 = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentChapter(Chapter newChapter)
    {
        currentChapter = newChapter;
        Debug.Log($"Current Chapter set to: {currentChapter}");
    }

    public bool IsChapter(Chapter chapter)
    {
        return currentChapter == chapter;
    }
}
