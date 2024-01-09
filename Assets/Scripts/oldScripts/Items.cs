using System.Collections.Generic;
using UnityEngine;

public abstract class Items : MonoBehaviour
{
    protected GameObject manager;
    protected List<string> interctDia = new List<string>(); //和物品interact的对话
    //public string description; //inventory, 物品description
    [HideInInspector] public bool canGoToInventory = false;
    string playerPrefSave;
    bool taken = false;

    //public int numAtInventory = -1;

    [HideInInspector] public bool isInteracting = false;
    protected bool canBeInteracted = false;
    [SerializeField] protected Sprite highlightedPic;
    protected Sprite originalPic;
    // [SerializeField] Sprite inventoryPic;

    protected SpriteRenderer rend;

    protected bool onObject;



    protected virtual void Start()
    {
        //manager = GameObject.FindGameObjectWithTag("Manager");
        rend = GetComponent<SpriteRenderer>();
        originalPic = rend.sprite;

        if (GetComponent<Collider2D>() == null)
        {
            Debug.Log("YOU FORGOT to add COLLIDER2D SHABI");
            // Add your logic here
        }

        CheckIfTaken();
    }

    private void CheckIfTaken()
    {
        playerPrefSave = gameObject.name + "Pref";
        if (PlayerPrefs.HasKey(playerPrefSave))
        {
            taken = PlayerPrefs.GetInt(playerPrefSave) == 1 ? true : false; // see whether the object has already been taken
        }

        if (taken)
        {
            Destroy(gameObject);
        }
    }

    ////路过highlight
    //protected virtual void OnCollisionEnter2D(Collision2D collision)
    //{
    //    onObject = true;
    //    //highlight it
    //    rend.sprite = highlightedPic;
    //    canBeInteracted = true;
    //}

    //protected virtual void OnCollisionExit2D(Collision2D collision)
    //{
    //    onObject = false;
    //    //cancel highlight
    //    rend.sprite = originalPic;
    //    canBeInteracted = false;
    //}



    protected virtual void Update()
    {
        //ray cast to detect item
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            onObject = true;
            //highlight it
            rend.sprite = highlightedPic;
            canBeInteracted = true;
        }
        else
        {
            onObject = false;
            //cancel highlight
            rend.sprite = originalPic;
            canBeInteracted = false;
        }

        if (canBeInteracted)
        {
            //it can be interacted again once player can move again
            if (InventoryStorage.instance.player.canMove == true) { isInteracting = false; }

            if (Input.GetKeyDown(KeyCode.E) && !isInteracting)
            {
                print("Interacting!");
                Interact();
                //put it into the inventory if it can be added
                if (canGoToInventory)
                {
                    //destroy the MIRROR child
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }

                    GoToInventory();
                }
            }
        }
    }

    protected virtual void Interact()
    {
        isInteracting = true;
        InventoryStorage.instance.display.Hide(); // hide display

        //add all related descriptions
        if(interctDia.Count > 0)
        {
            foreach (string str in interctDia)
            {
                manager.GetComponent<DialogueManager>().AddDialogue(str);
            }
        }
    }

    protected void GoToInventory()
    {
        InventoryStorage.instance.AddItem(name);
        PlayerPrefs.SetInt(playerPrefSave, 1); // set item as taken
        Destroy(gameObject);
    }
}
