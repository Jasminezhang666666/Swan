using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPuzzle1_box : Draggable
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Puzzle1_box")
        {
            print("Main Puzzle 1 Triggered!");
            Destroy(collision);
            SceneManager.LoadScene("Chp1_MainPuzzle1");
        }
    }
}
