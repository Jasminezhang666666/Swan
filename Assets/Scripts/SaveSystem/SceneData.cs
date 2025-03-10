using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public SceneDataSO Data;


    private void Awake()
    {
        GlobalManager.Instance.sceneData = this;
    }

    #region Save and Load

    public void Save(ref SceneSaveData data)
    {
        data.SceneID = Data.UniqueName;
    }

    public void Load( SceneSaveData data)
    {
        GlobalManager.Instance.sceneLoader.LoadSceneByIndex(data.SceneID);
    }

    #endregion

}

[System.Serializable]

public struct SceneSaveData
{
    public string SceneID;
}
