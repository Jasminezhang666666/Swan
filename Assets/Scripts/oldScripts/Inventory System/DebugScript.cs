using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    [SerializeField] KeyCode key1 = KeyCode.LeftControl;
    [SerializeField] KeyCode key2 = KeyCode.RightControl;

    bool isOpen = false;
    private void Update()
    {
        if (Input.GetKeyDown(key1) && Input.GetKeyDown(key2))
        {
            isOpen = !isOpen; 
            Debug.Log("Debugging = " + isOpen);
        }

        if(Input.GetKeyDown(KeyCode.Return) && isOpen)
        {
            isOpen = false;
        }

        transform.GetChild(0).GetComponent<Canvas>().enabled = isOpen;
    }

    public void InputProcess(string input)
    {
        if(input == "clear")
        {
            InventoryStorage.instance.Clear();
        }
        else
        {
            InventoryStorage.instance.AddItem(input);
        }
    }
}
