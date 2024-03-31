using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOne : MonoBehaviour
{

    public HitTiming hitTiming = HitTiming.None;

    [SerializeField] float spawnTime;
    [SerializeField] float crossingTime;
    [Space]
    [SerializeField] public float perfectWindowStart, perfectWindowEnd, goodWindowStart, goodWindowEnd, OKWindowStart, OKWindowEnd;

    RhythmManagerOne rm;
    Vector3 startPosition;
    Transform spawnTarget;
    float t = 0.0f;
    float duration = 0.0f;

    private void Awake()
    {
        spawnTarget = GameObject.FindGameObjectWithTag("SpawnTarget").GetComponent<Transform>();
        startPosition = transform.position;
    }

    public void SetSpawnItemInfo(RhythmManagerOne r, int st, int ct)
    {
        rm = r;
        spawnTime = (float)st;
        crossingTime = (float)ct;
        duration = (crossingTime - spawnTime) * 0.001f;

        OKWindowStart = crossingTime - rm.OkWindowMillis;
        OKWindowEnd = crossingTime + rm.OkWindowMillis;
        goodWindowStart = crossingTime - rm.GoodWindowMillis;
        goodWindowEnd = crossingTime + rm.GoodWindowMillis;
        perfectWindowStart = crossingTime - rm.PerfectWindowMillis;
        perfectWindowEnd = crossingTime + rm.PerfectWindowMillis;

        Debug.Log($"Spawn Time {spawnTime} || Crossing Time {crossingTime} || Move Duration: {duration}");
    }

    private void Update()
    {
        transform.position = Vector3.LerpUnclamped(startPosition, spawnTarget.position, t / duration);
        t += Time.deltaTime;
    }
}
