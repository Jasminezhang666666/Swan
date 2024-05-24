using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle2_Point : MonoBehaviour
{
    private Sprite originalSpr;
    [SerializeField] private Sprite chosedSpr;
    [SerializeField] private Sprite infoSpr; //give hint when clicked
    [SerializeField] public int index;
    private PointManager pointManager;
    private bool hinting = false;

    public bool chosed = false;

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
        if (Puzzle2_Hammer.HammerDragging) //with hammer
        {
            if (Input.GetMouseButtonDown(0)) // Detect mouse click
            {
                chosed = true;
                toChosedSpr();
                pointManager.CheckVadality(index);
                //Debug.Log("Point " + index + " has been chosen with hammer.");
            }
        }
        else if (Input.GetMouseButtonDown(0)) //without hammer, and clicking
        {
            StartCoroutine(GiveHint());
        }
    }

    private void OnMouseExit()
    {
        if (!hinting && !chosed)
        {
            toOriginalSpr();
        }
    }

    public void toOriginalSpr()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = originalSpr;
    }

    public void toChosedSpr()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = chosedSpr;
    }

    public void toInfoSpr()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = infoSpr;
    }

    private IEnumerator GiveHint() //之后再加东西，时间，不同的hint
    {
        hinting = true;
        toInfoSpr();
        Debug.Log("Giving hint for point: " + index);

        yield return new WaitForSeconds(2);

        toOriginalSpr();
        hinting = false;
        Debug.Log("Hint ended for point: " + index);
    }
}
