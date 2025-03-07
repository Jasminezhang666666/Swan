/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCharacter : MonoBehaviour
{
    // A unique identifier for this character.
    // Make sure each character prefab gets a unique id in the Inspector.
    [SerializeField]
    private string characterId;

    // List of scene names in which this character should persist.
    [SerializeField]
    private List<string> persistentScenes = new List<string>();

    // Static dictionary to track persistent character instances.
    private static Dictionary<string, PersistentCharacter> instances = new Dictionary<string, PersistentCharacter>();

    private void Awake()
    {
        // Check if an instance with this unique id already exists.
        if (instances.ContainsKey(characterId))
        {
            // A persistent instance already exists, so destroy this duplicate.
            Destroy(gameObject);
            return;
        }
        else
        {
            // No instance exists yet, so add this one to our dictionary.
            instances.Add(characterId, this);
            DontDestroyOnLoad(gameObject);
        }

        // Subscribe to scene loaded events.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Remove this instance from the dictionary if it’s our persistent instance.
        if (instances.ContainsKey(characterId) && instances[characterId] == this)
        {
            instances.Remove(characterId);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called whenever a new scene is loaded.
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        // If the new scene is not one of the scenes where this character should persist,
        // destroy this persistent instance.
        if (!persistentScenes.Contains(scene.name))
        {
            Destroy(gameObject);
        }
    }
}
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    // Public function to unload the "Rm_Stage01" scene.
    public void UnloadStageScene()
    {
        SceneManager.UnloadSceneAsync("Rm_Stage01");
    }
}
