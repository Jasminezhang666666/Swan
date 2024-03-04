using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Puzzle2_Point : MonoBehaviour
{
    private Sprite originalSpr;
    [SerializeField] private Sprite highlightedSpr;
    [SerializeField] private int index;
    private PointManager pointManager;

    public bool clicked = false;

    private void Start()
    {
        originalSpr = GetComponent<SpriteRenderer>().sprite;
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
            if (pointManager.CheckVadality(index))
            {
                clicked = true;
            }
            print("Point " + index + " has been clicked.");
        }
    }

    private void OnMouseExit()
    {
        toOriginalSpr();
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
