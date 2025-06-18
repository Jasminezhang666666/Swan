using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;


public class DanceMode : MonoBehaviour
{
    public static DanceMode instance;

    [Header("Rhythm Settings")]
    public string DanceModeFileName = "DanceRhythm.csv";
    public List<Rhythm> CorrectRhythm; //List of all rhythm
    public List<string> tempRhythm; //List of remain possible rhythm
    public string currentRhythm; //player input

    [Header("Click Settings")]
    float reflectTime = 0.1f; //0.1s mouse input detection for human reflection
    float clickTimer = 0f;
    bool leftClicked = false;
    bool rightClicked = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadRhythm();
        currentRhythm = string.Empty; //clean player input
        tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
    }

    private void Update()
    {
        GetInputRhythm();
        CheckInputRhythm();
    }

    /// <summary>
    /// load correct rhythm from file DanceMode
    /// </summary>
    void LoadRhythm()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, DanceModeFileName);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            // skip title row
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(',');

                if (parts.Length >= 2)
                {
                    Rhythm r = new Rhythm();
                    r.rhythmID = parts[0].Trim();
                    r.rhythmName = parts[1].Trim();
                    CorrectRhythm.Add(r);
                }
            }
        }
        else
        {
            Debug.LogError("File not found at path: " + filePath);
        }
    }

    /// <summary>
    /// Get player input rhythm from mouse
    /// </summary>
    void GetInputRhythm()
    {
 
        if (Input.GetMouseButtonDown(0)) // left key
        {
            leftClicked = true;
        }
        if (Input.GetMouseButtonDown(1)) // right key
        {
            rightClicked = true;
        }
        
        if (clickTimer > 0)
        {
            //start count when clicked
            if (leftClicked || rightClicked)
            {
                clickTimer -= Time.deltaTime;
            }
        }
        else
        {
            //check input in the limit time and reset input
            if (leftClicked && rightClicked)
            {
                currentRhythm += "2";
            }
            else if (leftClicked)
            {
                currentRhythm += "0";
            }
            else if (rightClicked)
            {
                currentRhythm += "1";
            }
            tempRhythm = tempRhythm.Where(s => s.StartsWith(currentRhythm)).ToList(); //delete unrelated rhythm
            leftClicked = false;
            rightClicked = false;
            clickTimer = reflectTime;
        }
    }

    /// <summary>
    /// check whether current input rhythm match correct rhythm
    /// </summary>
    void CheckInputRhythm()
    {
        if (tempRhythm.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (CorrectRhythm.Exists(r => r.rhythmID == currentRhythm))
                {
                    Rhythm result = CorrectRhythm.Find(r => r.rhythmID == currentRhythm);
                    Debug.Log(result.rhythmName);
                    ResetInputRhythm();
                } else
                {
                    Debug.Log("Wrong!!!!!!!!");
                    ResetInputRhythm();
                }
            }
        }
        else
        {
            Debug.Log("Wrong!!!!!!!!");
            ResetInputRhythm();
        }
    }

    void ResetInputRhythm()
    {
        tempRhythm = CorrectRhythm.Select(x => x.rhythmID).ToList();
        currentRhythm = null;
    }
}

[Serializable]
public struct Rhythm
{
    public string rhythmName;
    public string rhythmID; //correct input
    
}
