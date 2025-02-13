using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UIElements;
using System.Xml;
using static InvDatabase;
using UnityEngine.SceneManagement;

public class InventoryStorage : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public string selectedItemId;
    [SerializeField] AudioSource snd_open;

    // inventory bar display
    public InventoryDisplay display; // inv slot display script

    [SerializeField] InvDatabase databaseObj; // storage of all possible items
    Dictionary<string, Itm> databaseDict;
    private List<Itm> inventory = new List<Itm>(); // all the inventory items

    #region Singleton
    public static InventoryStorage instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "EndAnim")
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        // initialize dictionary
        databaseDict = databaseObj.GenerateDictionary(); // generate a dictionary
    }

    private void Update()
    {
        if (!player) // if player is missing, find it
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj)
            {
                player = playerObj.GetComponent<Player>();
            }
        }
    }

    // public methods
    public void RemoveItem(int indx) // remove item by index
    {
        inventory.RemoveAt(indx);
        display.Refresh();
    }

    public void Clear()
    {
        inventory.Clear();
        display.Refresh();
    }

    public void RemoveItem(string itemName) // remove item by name
    {
        Itm item = inventory.Find(x => x.id == itemName);

        if (item == null)
        {
            Debug.Log("Nothing removed");
        } else

        {
            inventory.Remove(item); // remove item from inventory
            display.Refresh();
        }
    }

    public void AddItem(string itemName) // add item by name 
    {
       if(inventory.Count < InventoryDisplay.NUMBER_OF_SLOTS)
        {
            Itm item = databaseDict[itemName];
            Debug.Log(item.id);
            if (item != null) // if item found
            {
                //play add sound
                snd_open.Play();

                inventory.Add(item); // add item to inventory
                display.Refresh();
            }
        } else
        {
            Debug.Log("Inventory full, item add failed.");
        }
    }

    public bool HasItem(string itemName)
    {
        Itm item = inventory.Find(x => x.id == itemName);
        Debug.Log(itemName + " was searched for");

        if (item == null)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public Itm At(int indx)
    {
        return inventory[indx];
    }

    public int Size()
    {
        return inventory.Count;
    }
}
