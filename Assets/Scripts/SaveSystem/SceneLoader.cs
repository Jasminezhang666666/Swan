using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO[] _sceneDataSOArray;
    private Dictionary<string, int> _sceneIDToIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        GlobalManager.Instance.sceneLoader = this;

        PopulateSceneMappings();
    }

    private void PopulateSceneMappings()
    {
        foreach(var sceneDataSO in _sceneDataSOArray)
        {
            _sceneIDToIndexMap[sceneDataSO.UniqueName] = sceneDataSO.SceneIndex;
        }
    }

    public void LoadSceneByIndex(string sceneIndexID)
    {
        if (_sceneIDToIndexMap.TryGetValue(sceneIndexID, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("No Scene found");
        }
    }

}
