using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Puzzle2_Point : MonoBehaviour
{
    private Sprite originalSpr;
    [SerializeField] private Sprite highlightedSpr;
    [SerializeField] public int index;
    private PointManager pointManager;

    public bool clicked = false;

    private void Awake()
    {
        originalSpr = GetComponent<SpriteRenderer>().sprite;
    }

    private void Start()
    {
        // Find the PointManager in the scene
        pointManager = FindObjectOfType<PointManager>();
        if (pointManager == null)
        {
            Debug.LogError("PointManager not found in the scene.");
        }
    }

    private void OnMouseOver()
    {
        toHighlightSpr();
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
            toHighlightSpr();
            pointManager.CheckVadality(index);
            //print("Point " + index + " has been clicked.");
        }
    }

    private void OnMouseExit()
    {
        if(!clicked)
        {
            toOriginalSpr();
        }
    }

    public void toOriginalSpr()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = originalSpr;
    }

    public void toHighlightSpr()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = highlightedSpr;
    }
}
