using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceMode : MonoBehaviour
{
    public static DanceMode instance;
    public List<string> CorrectRhythm; //List of all rhythm
    public List<string> tempRhythm; //List of remain possible rhythm
    public string currentRhythm; //player input

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentRhythm = string.Empty; //clean player input
        tempRhythm = new List<string>(CorrectRhythm); //copy all rhythm at start
    }

    private void Update()
    {
        GetInputRhythm();
        CheckInputRhythm();
    }

    /// <summary>
    /// Get player input rhythm from mouse
    /// </summary>
    void GetInputRhythm()
    {
        bool left = Input.GetMouseButtonDown(0);    // left key
        bool right = Input.GetMouseButtonDown(1);   // right key


        if (left && right)
        {
            currentRhythm += "2";
        }
        else if (left)
        {
            currentRhythm += "0";
        }
        else if (right)
        {
            currentRhythm += "1";
        }
    }

    /// <summary>
    /// check current input rhythm
    /// </summary>
    void CheckInputRhythm()
    {
        foreach (string key in tempRhythm)
        {


        }
    }
}
