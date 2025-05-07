using Fungus;
using MoonSharp.Interpreter.Debugging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewItem : MonoBehaviour
{
    //public Canvas canvas;
    public string description;
    public Sprite inventorySprite; //sprite show in inventory
    public GameObject targetInteractionPos; //item use place
    public string blockName;
    RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;
    Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (description == "")
        {
            Debug.LogWarning("Item " + name + "description is not added");
        }
        /*
        if (canvas == null)
        {
            Debug.LogError("Item " + name + "");
        }
        */
    }

    //for sprite item pick up
    private void OnMouseDown()
    {
        // disable the visible sprite so it disappears immediately
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        GoToInventory();
    }

    public void GoToInventory()
    {
        print("going into inventory");
        //gameObject.SetActive(false);
        //StartCoroutine(GoToInventoryAnimation());
        Inventory.instance.TurnOnOffInventory();
        Inventory.instance.AddInventoryList(
            name, 
            description, 
            (inventorySprite == null) ? image.sprite : inventorySprite, 
            targetInteractionPos, blockName
        ); //if inventorySprite is not provided, use image sprite
    }

    /*
    //UI-to-UI (not used anymore, but could be used in the future)
    IEnumerator GoToInventoryAnimation()
    {
        RectTransform targetUI = Inventory.instance.inventoryIcon;

        // 1. get start
        Vector3 startWorldPos = rectTransform.position;
        Vector2 startSize = rectTransform.sizeDelta;

        // 2. get target
        Vector3 targetWorldPos = targetUI.position;
        Vector2 targetSize = targetUI.sizeDelta;

        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            //world
            Vector3 worldPos = Vector3.Lerp(startWorldPos, targetWorldPos, t);
            Vector2 size = Vector2.Lerp(startSize, targetSize, t);

            //world to screen
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);

            //screen to target parent
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                screenPoint,
                null,
                out Vector2 localPoint
            );

            rectTransform.anchoredPosition = localPoint;
            rectTransform.sizeDelta = size;

            yield return null;
        }

        rectTransform.position = targetWorldPos;
        rectTransform.sizeDelta = targetSize;
        gameObject.SetActive(false);

        //add to inventory
       
        Inventory.instance.AddInventoryList(name, description, (inventorySprite == null) ? image.sprite : inventorySprite, targetInteractionPos, blockName); //if inventorySprite is not provided, use image sprite
        
    }
    */
}