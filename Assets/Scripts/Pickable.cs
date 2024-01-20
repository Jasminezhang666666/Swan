using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] public Item item;
    [SerializeField] private Sprite highlightSpr;
    [HideInInspector] public Sprite normalSpr;

    private void Awake()
    {
        normalSpr = GetComponent<SpriteRenderer>().sprite;

        //Debug Message:
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError("No BoxCollider2D attached to the GameObject");
        }
    }

    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().sprite = highlightSpr;
        if (Input.GetMouseButtonDown(0)) //鼠标右键捡起来
        {
            AddItemToInventory();
            //毁掉自己
            Destroy(gameObject);
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = normalSpr;
    }

    private void AddItemToInventory()
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            bool result = inventoryManager.AddItem(item);
            if(result == false)
            {
                Debug.Log("Inventory is full");
            }
        }
        else{
            Debug.LogError("InventoryManager not found in the scene.");
        }
    }
}
