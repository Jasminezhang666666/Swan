using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMask : MonoBehaviour
{
    [SerializeField] private GameObject note;
    private float extendRate;
    private bool keepExtending;
    public bool marked;
    
    void Start()
    {
        marked = false;
        extendRate = 5;
    }

    private void Update()
    {
        if (keepExtending)
        {
            Vector3 scale = transform.localScale;
            scale.x += extendRate * Time.deltaTime; 
            transform.localScale = scale;
        }
        
    }
    
    public void Extending()
    {
        keepExtending = !keepExtending;

    }
    public void StartExtending()
    {
        keepExtending = true;
    }
    
    public void StopExtending()
    {
        keepExtending = false;
    }

    public bool ReturnExtendingStatus()
    {
        return keepExtending;
    }
}
