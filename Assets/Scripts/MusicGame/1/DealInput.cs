using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DealInput : MonoBehaviour
{
    public static DealInput Instance { get; private set; }

    [SerializeField]
    private string fileName = "notes-1.txt";
    public string[,] notesForPlay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    void Start()
    {
        notesForPlay = ReadTxt();
    }

    private string[,] ReadTxt()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string fileContents = File.ReadAllText(filePath);
        string[] entry = fileContents.Split(";");
        string[,] notes = new string[entry.Length, 3];
        for(int i = 0;i<entry.Length; i++)
        {
            string[] pair = SplitPair(entry[i], ":");
            for(int j = 0; j < 2; j++)
            {
                notes[i, j] = pair[j].Replace("[", "").Replace("]", "").Trim();
            }
            string[] pairTime = SplitPair(notes[i, 1], ",");
            notes[i, 2] = pairTime[1].Replace("[", "").Replace("]", "").Trim();
            notes[i, 1] = pairTime[0].Replace("[", "").Replace("]", "").Trim();
        }
        return notes;
    }

    private string[] SplitPair(string inputStr, string character)
    {
        string[] pair = inputStr.Split(character);
        for (int j = 0; j < 2; j++)
        {
            pair[j] = pair[j].Replace("[", "").Replace("]", "").Trim();
        }
        return pair;
    }
}
