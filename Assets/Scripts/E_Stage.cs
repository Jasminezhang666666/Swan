using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// E_Stage extends EInteractable. 
/// When the player presses E, it loads the specified scene.
/// </summary>
public class E_Stage : EInteractable
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = "NextScene";

    public override void Interact()
    {
        // Optionally call base.Interact() if you still want to run the Fungus block 
        // or toggle a prefab from the original EInteractable logic.
        // If you do NOT want Fungus or prefab toggling here, you can remove base.Interact().
        base.Interact();

        // After (or instead of) Fungus logic, load the next scene.
        Debug.Log("E_Stage Interact: Loading scene " + sceneToLoad);
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
