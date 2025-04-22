using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public  SceneLoader sceneLoader;
    public SceneData sceneData;
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            // This will ensure that the GameManager is not destroyed when a new scene is loaded
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists and it's not this, destroy the duplicate GameObject
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            SaveManager.Save();
            Debug.Log("SAVING");
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            SaveManager.Load();
            Debug.Log("Loading");
        }

    }

    public void LoadSave()
    {
        SaveManager.Load();
        Debug.Log("Loading");
    }
}
