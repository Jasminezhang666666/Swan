using System;
using System.Collections;
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
    Image image;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
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

    // Keep existing UI-to-UI method unchanged
    public void GoToInventory() => StartCoroutine(GoToInventoryAnimation());
    private IEnumerator GoToInventoryAnimation()
    {
        RectTransform targetUI = Inventory.instance.inventoryIcon;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Image image = GetComponent<Image>();

        Vector3 startWorldPos = rectTransform.position;
        Vector2 startSize = rectTransform.sizeDelta;
        Vector3 targetWorldPos = targetUI.position;
        Vector2 targetSize = targetUI.sizeDelta;

        float elapsed = 0f, duration = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Vector3 worldPos = Vector3.Lerp(startWorldPos, targetWorldPos, t);
            Vector2 size = Vector2.Lerp(startSize, targetSize, t);

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                screenPoint, null,
                out Vector2 localPoint
            );
            rectTransform.anchoredPosition = localPoint;
            rectTransform.sizeDelta = size;
            yield return null;
        }

        rectTransform.position = targetWorldPos;
        rectTransform.sizeDelta = targetSize;
        gameObject.SetActive(false);

        Inventory.instance.AddInventoryList(
            name, description,
            inventorySprite ?? image.sprite,
            targetInteractionPos, blockName
        );
        Inventory.instance.TurnOnOffInventory();
    }

    /// <summary>
    /// Animate a temporary UI icon from world-space position into the inventory.
    /// </summary>
    public void GoToInventoryFromWorld(Vector3 worldPosition, Action onComplete)
    {
        StartCoroutine(GoToInventoryFromWorldCoroutine(worldPosition, onComplete));
    }

    private IEnumerator GoToInventoryFromWorldCoroutine(Vector3 worldPosition, Action onComplete)
    {
        // Create a temporary UI Image under the same parent as the inventory icon
        RectTransform iconParent = Inventory.instance.inventoryIcon.parent as RectTransform;
        GameObject tempGO = new GameObject(name + "_tempIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        tempGO.transform.SetParent(iconParent, false);
        Image tempImage = tempGO.GetComponent<Image>();
        tempImage.sprite = inventorySprite;
        RectTransform tempRect = tempGO.GetComponent<RectTransform>();

        // Set initial position and size at world point
        Vector2 startScreen = RectTransformUtility.WorldToScreenPoint(null, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            iconParent, startScreen, null, out Vector2 startLocal
        );
        tempRect.anchoredPosition = startLocal;
        Vector2 startSize = new Vector2(100, 100);
        tempRect.sizeDelta = startSize;

        // Target UI position and size
        RectTransform targetUI = Inventory.instance.inventoryIcon;
        Vector2 targetLocal = targetUI.anchoredPosition;
        Vector2 targetSize = targetUI.sizeDelta;

        float elapsed = 0f, duration = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            tempRect.anchoredPosition = Vector2.Lerp(startLocal, targetLocal, t);
            tempRect.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            yield return null;
        }

        // Finalize and cleanup
        tempRect.anchoredPosition = targetLocal;
        tempRect.sizeDelta = targetSize;
        Destroy(tempGO);

        // Add to inventory
        Inventory.instance.AddInventoryList(
            name, description,
            inventorySprite, targetInteractionPos, blockName
        );
        Inventory.instance.TurnOnOffInventory();

        onComplete?.Invoke();
    }
}
