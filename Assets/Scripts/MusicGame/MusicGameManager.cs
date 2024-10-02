using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public enum musicNotesPosition
{
    A,
    B,
    C,
    D,
    E
}

public enum musicNoteType
{
    Long,
    Short
}
public class MusicGameManager : MonoBehaviour
{
    public static MusicGameManager Instance { get; private set; }
    //how many lines of node
    public int mode = 1;
    private Vector2[] spawnPositions;
    [SerializeField]private int positionsAmt;
    [SerializeField]private Vector2 defaultPosition; 
    public GameObject notePrefab;
    public GameObject shortNote;
    private GameObject newNote;
    private GameObject newNoteA;
    private GameObject newNoteB;
    private GameObject newNoteC;
    private GameObject newNoteD;
    private GameObject newNoteE;

    private float upperHeight;
    private float lowerHeight;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        spawnPositions = new Vector2[positionsAmt];
        for (int i = 0; i < positionsAmt; i++)
        {
            spawnPositions[i] = defaultPosition + new Vector2(0, 2 * i);
        }

        upperHeight = GameObject.Find("upperRing").transform.position.y;
        lowerHeight = GameObject.Find("lowerRing").transform.position.y;

    }
    private Vector2 MatchPosition(musicNotesPosition posName)
    {
        Vector2 currentPosition;
        switch (posName)
        {
            case musicNotesPosition.A:
                if (mode == 1)
                {
                    currentPosition = spawnPositions[2];
                }else if (mode == 2)
                {
                    currentPosition = new Vector2(defaultPosition.x, upperHeight);
                }
                else
                {
                    currentPosition = spawnPositions[0];
                }
                break;
            case musicNotesPosition.B:
                if (mode == 2)
                {
                    currentPosition = new Vector2(defaultPosition.x, lowerHeight);
                }
                else if(mode == 3)
                {
                    currentPosition = spawnPositions[2];
                }
                else
                {
                    currentPosition = spawnPositions[1];
                }
                break;
            case musicNotesPosition.C:
                if (mode == 3)
                {
                    currentPosition = spawnPositions[3];
                }
                else
                {
                    currentPosition = spawnPositions[2];
                }
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
    
    public void SpawnNote(musicNotesPosition pos)
    {
        GameObject newNote = null;
        Vector2 spawnPosition = MatchPosition(pos);
        
        switch (pos)
        {
            case musicNotesPosition.A:
                newNote = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
                newNote.transform.Find("Note").GetComponent<NotesMoving>().SetType(musicNoteType.Long);
                newNote.transform.Find("Note").GetComponent<NotesMoving>().SetPos(musicNotesPosition.A);
                newNoteA = newNote; 
                break;
            case musicNotesPosition.B:
                newNote = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
                newNoteB = newNote; 
                newNote.transform.Find("Note").GetComponent<NotesMoving>().SetType(musicNoteType.Long);
                newNote.transform.Find("Note").GetComponent<NotesMoving>().SetPos(musicNotesPosition.B);
                break;
            default:
                throw new Exception("Invalid music note position");
        }
        
        if (newNote != null)
        {
            var noteMovingComponent = newNote.transform.Find("Note").GetComponent<NotesMoving>();
            noteMovingComponent.StartExtending();
            noteMovingComponent.SetType(musicNoteType.Long); // Set to Long note type
        }
    }

    
    public void StopSpawnNote(musicNotesPosition pos)
    {
        GameObject newNote;
        switch (pos)
        {
            case musicNotesPosition.A:
                newNote = newNoteA;
                break;
            case musicNotesPosition.B:
                newNote = newNoteB;
                break;
            default:
                newNote = newNoteA;
                throw new Exception("Note random");
        }
        newNote.transform.Find("Note").GetComponent<NotesMoving>().StopExtending();
    }
    
    public void SpawnShortNote(musicNotesPosition pos, musicNoteType type)
    {
        Vector2 spawnPosition = MatchPosition(pos);
        newNote = Instantiate(shortNote, spawnPosition, Quaternion.identity);
        newNote.transform.Find("Note").GetComponent<NotesMoving>().SetType(musicNoteType.Short);
        newNote.transform.Find("Note").GetComponent<NotesMoving>().SetPos(pos);
    }
    
    

    

}
