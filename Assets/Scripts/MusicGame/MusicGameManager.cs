using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum musicNotesPosition
{
    A,
    B,
    C,
    D,
    E
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
    
    void SpawnNote(musicNotesPosition posName)
    {
        Vector2 spawnPosition = MatchPosition(posName);
        GameObject newNote = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnNodeA()
    {
        Vector2 spawnPosition = MatchPosition(musicNotesPosition.A);
        newNoteA = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNoteA.transform.Find("Note").GetComponent<NotesMoving>().StartExtending();
    }
    
    public void StopSpawnNodeA()
    {
        newNoteA.transform.Find("Note").GetComponent<NotesMoving>().StopExtending();
    }
    
    public void SpawnNodeB()
    {
        Vector2 spawnPosition = MatchPosition(musicNotesPosition.B);
        newNoteB = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNoteB.transform.Find("Note").GetComponent<NotesMoving>().StartExtending();
    }
    
    public void StopSpawnNodeB()
    {
        newNoteB.transform.Find("Note").GetComponent<NotesMoving>().StopExtending();
    }
    
    public void SpawnNodeC()
    {
        Vector2 spawnPosition = MatchPosition(musicNotesPosition.C);
        newNoteC = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNoteC.transform.Find("Note").GetComponent<NotesMoving>().StartExtending();
    }
    
    public void StopSpawnNodeC()
    {
        newNoteC.GetComponent<NotesMoving>().StopExtending();
    }
    
    public void SpawnNodeD()
    {
        Vector2 spawnPosition = MatchPosition(musicNotesPosition.D);
        newNoteD = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNoteD.GetComponent<NotesMoving>().StartExtending();
    }
    
    public void StopSpawnNodeD()
    {
        newNoteD.GetComponent<NotesMoving>().StopExtending();
    }
    
    public void SpawnNodeE()
    {
        Vector2 spawnPosition = MatchPosition(musicNotesPosition.E);
        newNoteE = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        newNoteE.GetComponent<NotesMoving>().StartExtending();
    }
    
    public void StopSpawnNodeE()
    {
        newNoteE.GetComponent<NotesMoving>().StopExtending();
    }
    

}
