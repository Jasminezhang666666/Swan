using UnityEngine;
using UnityEngine.SceneManagement;

public class E_Stage : EInteractable
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "NextScene";

    public override void Interact()
    {
        base.Interact();

        // Mark that the player has now looked at the stage in Chapter 1
        ChapterManager.Instance.Chp1_LookedAtStage = true;

        // Then load the next scene (if assigned)
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("E_Stage Interact: Loading scene " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene specified to load.");
        }
    }
}
