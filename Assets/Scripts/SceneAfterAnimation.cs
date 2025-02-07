using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAfterAnimation : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad; // Name of the scene to load after the animation completes.

    /// <summary>
    /// This method is intended to be called via an Animation Event at the end of the animation.
    /// </summary>
    public void OnAnimationFinished()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("SceneAfterAnimation: No scene specified to load.");
        }
    }
}