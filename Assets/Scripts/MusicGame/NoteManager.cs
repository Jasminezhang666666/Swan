using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum musicNotesPosition
{
    A,
    B,
    C,
    D,
    E
}
public class NoteManager : MonoBehaviour
{

    public GameObject notePrefab; 
    public float spawnInterval = 5f; 
    private Vector2 defaultPosition = new Vector2(-12, -4); 

    private float nextSpawnTime;
    private float currentlength = 2f;

    private string[,] musicScript;
    [SerializeField]
    private int positionsAmt = 5;
    private Vector2[] spawnPositions;
    private float[] listOfStartTime;
    private float[] listOfDuration;
    private musicNotesPosition[] listOfNotes;

    [SerializeField] private float StartOffset = 2.2f;

    public AudioSource songToPlay;

    void Start()
    {
        songToPlay.Play();
        musicScript = DealInput.Instance.notesForPlay;
        spawnPositions = new Vector2[positionsAmt];
        
        for (int i = 0; i < positionsAmt; i++)
        {
            spawnPositions[i] = defaultPosition + new Vector2(0, 2 * i);
        }
        listOfStartTime = ConvertMusicScript<float>(musicScript, 1);
        listOfDuration = ConvertMusicScript<float>(musicScript, 2);
        listOfNotes = ConvertMusicScript<musicNotesPosition>(musicScript, 0);

        StartCoroutine(Timer());
        //nextSpawnTime = Time.time + spawnInterval;
    }

    IEnumerator Timer()
    {
        int currentNote = 0;
        int finalNote = musicScript.GetLength(0);

        float currentTime = StartOffset;
        while (currentNote < finalNote)
        {
            float targetSpawnTime = listOfStartTime[currentNote];
            if (currentTime >= targetSpawnTime)
            {
                SpawnNote(listOfNotes[currentNote], listOfDuration[currentNote]);
                currentNote++;
            }
            yield return new WaitForSeconds(0.01f);
            currentTime += 0.01f;
        }
        print("final " + currentTime);
        //songToPlay.Stop();
    }

    //public void StartMusic()
    //{
    //    musicScript = DealInput.Instance.notesForPlay;
    //    nextSpawnTime = Time.time + spawnInterval;
    //}

    void Update()
    {
        //if (Time.time >= nextSpawnTime)
        //{
        //    SpawnNote(currentlength);
        //    nextSpawnTime = Time.time + spawnInterval;
        //}
    }

    void SpawnNote(musicNotesPosition posName, float existingTime)
    {
        Vector2 spawnPosition = MatchPosition(posName);
        //Vector2 spawnPosition = defaultPosition + new Vector2(0, Random.Range(-4, 4));
        GameObject newNote = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNote.GetComponent<NotesMoving>().Initialize(existingTime);
    }

    private Vector2 MatchPosition(musicNotesPosition posName)
    {
        Vector2 currentPosition;
        switch (posName)
        {
            case musicNotesPosition.A:
                currentPosition = spawnPositions[0];
                break;
            case musicNotesPosition.B:
                currentPosition = spawnPositions[1];
                break;
            case musicNotesPosition.C:
                currentPosition = spawnPositions[2];
                break;
            case musicNotesPosition.D:
                currentPosition = spawnPositions[3];
                break;
            case musicNotesPosition.E:
                currentPosition = spawnPositions[4];
                break;
            default:
                currentPosition = spawnPositions[0];
                break;
        }
        return currentPosition;

    }

    public T[] ConvertMusicScript<T>(string[,] script, int index)
    {
            try
            {
                T[] listOfT = new T[script.GetLength(0)];
                for (int i = 0; i < script.GetLength(0); i++)
                {
                    if (typeof(T) == typeof(float))
                    {
                        listOfT[i] = (T)Convert.ChangeType(ParseFraction(script[i, index]), typeof(T));
                    }
                    else if (typeof(T) == typeof(musicNotesPosition))
                    {
                        listOfT[i] = (T)Convert.ChangeType(Enum.Parse(typeof(musicNotesPosition), script[i, index], true), typeof(T));
                    }
                }
                return listOfT;
            }
            catch (Exception e)
            {
                throw new Exception("CONVERT SCRIPT FAILURE" + e);
            }
    }


    public float ParseFraction(string fraction)
    {
        float returnVal = 0;
        string[] parts = fraction.Split('/');
        if (parts.Length == 2)
        {
            float numerator;
            float denominator;

            if (float.TryParse(parts[0], out numerator) && float.TryParse(parts[1], out denominator))
            {
                if (denominator != 0)
                {
                    return numerator / denominator;
                }
            }
        }
        return returnVal;
    }
}
