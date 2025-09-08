using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Block
{
    public string colorName;   // display color name
    public string inputID;     // e.g. "0102"
    public string colorHex;     // e.g. "#ff6b6b"
}

public struct Rhythm 
{
    public string rhythmName;
    public List<string> blocks;
}


public class RhythmChecker : MonoBehaviour
{
    [Header("File Settings")]
    [Tooltip("CSV in StreamingAssets with columns: ID,Name,ColorHex")]
    public string BlockFileName = "DanceBlock.csv";
    [Tooltip("CSV in StreamingAssets with columns: ID,Name,ColorHex")]
    public string RhythmFileName = "DanceBlock.csv";

    [Header("Click Settings")]
    [Tooltip("Debounce window for human reaction (seconds)")]
    [SerializeField] private float reflectTime = 0.1f;

    // Public, readable state
    public List<Block> CorrectRhythm;
    public List<string> tempRhythm;
    public string currentRhythm = string.Empty;
    public bool InputReceivedThisBeat = false;

    // Internals
    private float clickTimer = 0f;

    private void Awake()
    {
        LoadRhythm();
        ResetAll();
    }

    public void ResetAll()
    {
        InputReceivedThisBeat = false;
        clickTimer = 0f;
        tempRhythm = CorrectRhythm.Select(x => x.inputID).ToList();
        currentRhythm = string.Empty;
    }

    /// <summary>Clear feedback & matching buffers, but do NOT touch timers/indicators.</summary>
    public void SoftReset()
    {
        InputReceivedThisBeat = false;
        tempRhythm = CorrectRhythm.Select(x => x.inputID).ToList();
        currentRhythm = string.Empty;
    }

    /// <summary>Call at the start of each beat window to allow one attempt per beat.</summary>
    public void StartNewBeatWindow()
    {
        InputReceivedThisBeat = false;
    }

    /// <summary>
    /// Reads mouse input ONCE per beat. Returns:
    /// - true when a click was consumed; sets out params:
    ///     - inBuffer: whether within timing window (manager decides the window)
    ///     - timerAtClick: current beat timer for UI finish animation
    /// - false when no click happened this frame.
    /// </summary>
    public bool TryConsumeInput(bool inBuffer, float beatTimer, out bool inBufferOut, out float timerAtClick)
    {
        inBufferOut = false;
        timerAtClick = 0f;

        // cooldown
        if (clickTimer > 0f) clickTimer -= Time.deltaTime;

        // only one attempt per beat
        if (InputReceivedThisBeat) return false;

        // only consume if cooldown elapsed
        if (clickTimer > 0f) return false;

        // read downs
        bool l = Input.GetMouseButtonDown(0);
        bool r = Input.GetMouseButtonDown(1);
        if (!l && !r) return false;

        InputReceivedThisBeat = true;    // lock this beat
        clickTimer = reflectTime;
        inBufferOut = inBuffer;
        timerAtClick = Mathf.Max(0f, beatTimer);

        // build rhythm symbol
        if (l && r) currentRhythm += "2";
        else if (l) currentRhythm += "0";
        else currentRhythm += "1";

        // prune candidate list
        tempRhythm = tempRhythm.Where(s => s.StartsWith(currentRhythm)).ToList();

        return true;
    }

    /// <summary>Try get matching rhythm entry for the full combo (e.g., 4 inputs).</summary>
    public bool TryGetExactMatch(out Block match)
    {
        match = default;
        if (CorrectRhythm.Exists(r => r.inputID == currentRhythm))
        {
            match = CorrectRhythm.Find(r => r.inputID == currentRhythm);
            return true;
        }
        return false;
    }

    private void LoadRhythm()
    {
        CorrectRhythm.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, BlockFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("[RhythmChecker] CSV not found: " + path);
            return;
        }

        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length >= 2)
            {
                string id = parts[0].Trim().Trim('\uFEFF');
                string name = parts[1].Trim();
                string hex = (parts.Length >= 3 ? parts[2] : "#ffffff").Trim();

                CorrectRhythm.Add(new Block { inputID = id, colorName = name, colorHex = hex });
            }
            else
            {
                Debug.LogWarning($"[RhythmChecker] Malformed line {i + 1}");
            }
        }
    }
}
