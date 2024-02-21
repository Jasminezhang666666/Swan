using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPuzzle1_box : Draggable
{
    private void Start()
    {
        highlightSpr = gameObject.GetComponent<Draggable>().highlightSpr;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Puzzle1Trigger")
        {
            print("Main Puzzle 1 Triggered!");
            Destroy(collision.gameObject);
            StartCoroutine(WaitAndLoadScene("Chp1_MainPuzzle1", 1.0f));
        }
    }

    IEnumerator WaitAndLoadScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
