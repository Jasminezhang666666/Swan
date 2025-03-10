using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager 
{

    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        //This is all the things that will be saved
        public SceneSaveData sceneSaveData;
    }


    //Helper method to save data
    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    //Actual saving function
    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    //This is where save is happening
    private static void HandleSaveData()
    {
        GlobalManager.Instance.sceneData.Save(ref _saveData.sceneSaveData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GlobalManager.Instance.sceneData.Load(_saveData.sceneSaveData);
    }



}
