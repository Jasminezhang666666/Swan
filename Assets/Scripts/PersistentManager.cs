using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    // Public function to unload the "Rm_Stage01" scene.
    public void UnloadStageScene()
    {
        // Optionally re-enable visuals before unloading if needed.
        EnableBackstageVisuals();
        SceneManager.UnloadSceneAsync("Rm_Stage01");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When Rm_Stage01 is loaded additively, disable the visuals in Rm_BackStage01.
        if (scene.name == "Rm_Stage01")
        {
            DisableBackstageVisuals();
            Debug.Log("Rm_Stage01 loaded: disabling backstage visuals in Rm_BackStage01.");
        }
    }

    public void DisableBackstageVisuals()
    {
        // Get the backstage scene by name.
        Scene backstageScene = SceneManager.GetSceneByName("Rm_BackStage01");
        if (!backstageScene.IsValid())
        {
            Debug.LogWarning("Rm_BackStage01 is not loaded or valid.");
            return;
        }

        // Iterate through the root GameObjects of Rm_BackStage01.
        GameObject[] rootObjects = backstageScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            // Assuming you’ve tagged the visuals with "BackstageVisual"
            if (obj.CompareTag("DisableVisual"))
            {
                obj.SetActive(false);
                Debug.Log($"Disabled backstage visual: {obj.name}");
            }
        }
    }

    public void EnableBackstageVisuals()
    {
        // Get the backstage scene by name.
        Scene backstageScene = SceneManager.GetSceneByName("Rm_BackStage01");
        if (!backstageScene.IsValid())
        {
            Debug.LogWarning("Rm_BackStage01 is not loaded or valid.");
            return;
        }

        GameObject[] rootObjects = backstageScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj.CompareTag("BackstageVisual"))
            {
                obj.SetActive(true);
                Debug.Log($"Enabled backstage visual: {obj.name}");
            }
        }
    }
}
