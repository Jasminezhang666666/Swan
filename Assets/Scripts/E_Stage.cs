using UnityEngine;
using UnityEngine.SceneManagement;

public class E_Stage : EInteractable
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "NextScene";

    public override void Interact()
    {
        base.Interact();

        // Get the current scene's name.
        string currentScene = SceneManager.GetActiveScene().name;

        // Set the appropriate ChapterManager flag based on the current scene.
        if (currentScene == "Rm_BackStage01")
        {
            ChapterManager.Instance.Chp1_LookedAtStage = true;
        }
        else if (currentScene == "Rm_DressingRoom02")
        {
            ChapterManager.Instance.Chp1_PlayedPuzzle1 = true;
        }

        // Load the next scene if a scene name is provided.
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene specified to load.");
        }
    }
}
