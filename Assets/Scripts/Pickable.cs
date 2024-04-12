using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] public Item item;  // Item data
    [SerializeField] private Sprite highlightSpr;  // Highlight sprite on mouse over
    [HideInInspector] public Sprite normalSpr;  // Normal sprite

    private void Awake()
    {
        normalSpr = GetComponent<SpriteRenderer>().sprite;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError("No BoxCollider2D attached to the GameObject");
        }
    }

    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().sprite = highlightSpr;
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            AddItemToFanInventory();
            Destroy(gameObject);  // Destroy the pickable object after it has been picked up
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = normalSpr; // Revert to the normal sprite when the mouse exits
    }

    private void AddItemToFanInventory()
    {
        FanInventoryManager fanInventoryManager = FindObjectOfType<FanInventoryManager>();
        if (fanInventoryManager != null)
        {
            FanItem fanItem = gameObject.AddComponent<FanItem>();  // Add FanItem dynamically
            fanItem.SetupItem(item);  // Setup the FanItem with the picked item
            fanInventoryManager.AddItem(fanItem);  // Add to inventory
        }
        else
        {
            Debug.LogError("FanInventoryManager not found in the scene.");
        }
    }
}
