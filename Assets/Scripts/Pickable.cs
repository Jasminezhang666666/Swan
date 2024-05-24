using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] private Item item;  // Item data
    [SerializeField] private Sprite highlightSpr;  // Highlight sprite on mouse over
    [SerializeField] private FanItem fanItem;  // Reference to the existing FanItem in the scene
    [HideInInspector] public Sprite normalSpr;  // Normal sprite

    private void Awake()
    {
        normalSpr = GetComponent<SpriteRenderer>().sprite;
        // Ensure there's a collider for mouse interactions.
        if (!GetComponent<BoxCollider2D>())
        {
            Debug.LogError("No BoxCollider2D attached to the GameObject");
        }
        // Initially, fanItem should be deactivated.
        if (fanItem != null)
        {
            fanItem.gameObject.SetActive(false);
        }
    }

    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().sprite = highlightSpr;
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            AddItemToFanInventory();
        }
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = normalSpr; // Revert to the normal sprite when the mouse exits
    }

    private void AddItemToFanInventory()
    {
        if (fanItem != null)
        {
            fanItem.SetupItem(item);  // Setup the FanItem with the picked item data
            //fanItem.SetHighlightSprite(highlightSpr); 
            fanItem.gameObject.SetActive(true);  // Activate the FanItem

            FanInventoryManager fanInventoryManager = FindObjectOfType<FanInventoryManager>();
            if (fanInventoryManager != null)
            {
                fanInventoryManager.AddItem(fanItem);  // Add to inventory
                gameObject.SetActive(false); // Optionally deactivate the pickable GameObject
            }
            else
            {
                Debug.LogError("FanInventoryManager not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("FanItem reference not set on " + gameObject.name);
        }
    }

    public void Reactivate()
    {
        this.gameObject.SetActive(true); // Reactivate the pickable GameObject
        if (fanItem != null)
        {
            fanItem.gameObject.SetActive(false); // Deactivate the FanItem as it's being 'put back'
        }
    }
}
