using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DealInput : MonoBehaviour
{
    private string fileName = "notes-1.txt";
    void Start()
    {
        ReadTxt();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ReadTxt()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string fileContents = File.ReadAllText(filePath);
        string[] rows = fileContents.Split(";");
        foreach(string i in rows)
        {
            print(i);
        }
    }
}
