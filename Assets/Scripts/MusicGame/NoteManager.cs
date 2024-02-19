using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public GameObject notePrefab; 
    public float spawnInterval = 5f; 
    private Vector2 defaultPosition = new Vector2(-12, 0); 

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnNote();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnNote()
    {
        Vector2 spawnPosition = defaultPosition + new Vector2(0, Random.Range(-4, 4));
        Instantiate(notePrefab, spawnPosition, Quaternion.identity);
    }
}
