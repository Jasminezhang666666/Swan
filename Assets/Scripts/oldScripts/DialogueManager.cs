using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel; // Reference to the UI panel

    private List<string> dialogue = new List<string>();

    private int index = 0;
    private InventoryDisplay inventoryDisplay;

    private void Start()
    {
        inventoryDisplay = InventoryStorage.instance.display; // inventory display game object
    }

    private void Update()
    {
        if (dialogue.Count > 0)
        {
            inventoryDisplay.Hide();
            InventoryStorage.instance.player.canMove = false;

            //Press mouse button/Space button to display next text
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (index < dialogue.Count)
                {
                    DisplayNextSentence();
                }
                else
                {
                    ClearDialogue();
                    Debug.Log("End of dialogue.");
                }
            }
        }
    }

    private void DisplayNextSentence()
    {
        dialogueText.text = dialogue[index];
        index++;
    }

    // Add dialogue to the lists
    public void AddDialogue(string sentence)
    {
        dialogue.Add(sentence);
        if(dialogue.Count == 1) //display the first dialogue
        {
            DisplayNextSentence();
        }
    }

    //clear all related dialogue list
    private void ClearDialogue() {
        InventoryStorage.instance.player.canMove = true;

        dialogue.Clear();
        dialogueText.text = null;
        index = 0;

        //Get Inventory back again
        inventoryDisplay.Show();
        InventoryStorage.instance.selectedItemId = null; // remove selected item after dialogue
    }

}

