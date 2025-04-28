using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    Player player;
    public Flowchart flowchart;

    public GameObject inventory; //whole inventory
    public GameObject inventoryListParent; //inventory item list
    public RectTransform inventoryIcon; //left bottom cornor ui

    public GameObject itemDetails; //details parent contains following attributs
    public Image itemImage; //item image
    public TextMeshProUGUI itemDescription; //item description
    public TextMeshProUGUI itemName; //item name
    public NewInventoryItem currentItem; //current item shown in inventory
    [HideInInspector]public bool usingCurrentItem;

    [HideInInspector] public Vector2 inventoryIconScreenPos;
    public List<NewInventoryItem> items = new List<NewInventoryItem>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckPlayerInstance();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TurnOnOffInventory();
        }
        */
        if (usingCurrentItem)
        {
            ItemFollowMouse();
        }
    }

    /// <summary>
    /// switch inventory active status
    /// </summary>
    public void TurnOnOffInventory()
    {
        if (items.Count == 0)
        {
            itemDetails.SetActive(false);
        }
        else
        {
            itemDetails.SetActive(true);
        }
        if (currentItem == null && items.Count > 0)
        {
            currentItem = items[0];
        }
        CheckPlayerInstance();
        player.canMove = inventory.activeSelf;
        //Debug.Log(player.canMove);
        inventory.SetActive(!inventory.activeSelf);
    }

    /// <summary>
    /// if player == null, find it again
    /// </summary>
    void CheckPlayerInstance()
    {
        // Find the player by tag if not assigned.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
            }
            else
            {
                Debug.LogError("Player not found! Please ensure the player has the 'Player' tag.");
            }
        }
    }

    /// <summary>
    /// add an item to inventory
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="sprite"></param>
    public void AddInventoryList(string name, string description, Sprite sprite, GameObject targetInteractionPos, string blockName)
    {
        NewInventoryItem item = new NewInventoryItem(name, description, sprite, targetInteractionPos, blockName);
        items.Add(item);
        item.itemObject.transform.SetParent(inventoryListParent.transform);

        Button btn = item.itemObject.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            itemName.text = item.itemName;
            itemDescription.text = item.itemDescription;
            itemImage.sprite = item.itemSprite;
            itemImage.color = Color.white;
            currentItem = item;
            UseItem();
        });
    }

    public void UseItem()
    {
        //TurnOnOffInventory();
        usingCurrentItem = true;
        currentItem.itemObject.transform.SetParent(gameObject.transform);
    }

    void ItemFollowMouse()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out localPoint
        );
        currentItem.itemObject.localPosition = localPoint;

        CheckTargetInteraction();
    }

    /// <summary>
    /// Check if it can interact with specific place
    /// </summary>
    void CheckTargetInteraction()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D cld = currentItem.targetPlace.GetComponent<Collider2D>();
        if (cld.OverlapPoint(mousePos))
        {
            currentItem.itemObject.GetComponent<Image>().color = Color.green;
            //correct
            if (Input.GetMouseButtonDown(0))
            {
                //call corresponding block
                if (!string.IsNullOrEmpty(currentItem.blockName) && flowchart != null)
                {
                    if (!flowchart.HasExecutingBlocks())
                    {
                        Debug.Log("Executing block: " + currentItem.blockName);
                        flowchart.ExecuteBlock(currentItem.blockName);
                    }
                }
                else
                {
                    Debug.LogWarning("No blockName provided or Flowchart not assigned!");
                }

                items.Remove(currentItem);
                Destroy(currentItem.itemObject.gameObject);
                currentItem = null;
                usingCurrentItem = false;
            }
        } else
        {
            currentItem.itemObject.GetComponent<Image>().color = Color.white;
            if (Input.GetMouseButtonDown(0))
            {
                currentItem.itemObject.SetParent(inventoryListParent.transform);
                usingCurrentItem = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            currentItem.itemObject.SetParent(inventoryListParent.transform);
            usingCurrentItem = false;
        }
    }
}

[Serializable]
public class NewInventoryItem
{
    public RectTransform itemObject;
    public GameObject targetPlace;
    
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public string blockName;
    
    public NewInventoryItem(string name, string description, Sprite sprite, GameObject target, string blockName)
    {
        itemName = name;
        itemDescription = description;
        itemSprite = sprite;
        this.blockName = blockName;
        
        //create object
        GameObject item = new GameObject();
        item.name = name;
        Image itemImage = item.AddComponent<Image>();
        itemImage.sprite = sprite;
        itemObject = item.GetComponent<RectTransform>();

        targetPlace = target;
        //Button itemBt = itemObject.AddComponent<Button>();
    }


}
