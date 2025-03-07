using UnityEngine;
using UnityEngine.SceneManagement;

public class E_Stage : EInteractable
{
    [Header("Scene Settings")]
    // This should be set to "Rm_Stage01" in the Inspector.
    [SerializeField] private string sceneToLoad = "Rm_Stage01";

    public override void Interact()
    {
        base.Interact();

        // Get the current scene's name.
        string currentScene = SceneManager.GetActiveScene().name;

        // Set flags based on the current scene.
        if (currentScene == "Rm_BackStage01")
        {
            ChapterManager.Instance.Chp1_LookedAtStage = true;
        }
        else if (currentScene == "Rm_DressingRoom02")
        {
            ChapterManager.Instance.Chp1_PlayedPuzzle1 = true;
        }

        // Load the next scene additively.
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogWarning("No scene specified to load.");
        }
    }
}