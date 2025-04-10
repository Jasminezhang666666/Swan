using Fungus;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawing : MonoBehaviour
{
    RawImage drawLayer; //board to draw
    RectTransform drawRect;

    public Flowchart flowchart; // Reference to the Fungus Flowchart
    public string blockName;    // The name of the specific block to trigger for this interactable

    [Header("DrawBoard Settings")]
    public Color drawColor = Color.black;
    public int brushSize = 2;
    public Color boardColor = new Color(0, 0, 0, 0); //default transparent



    private Texture2D drawTexture; //new texture

    private Vector2? lastDrawPos = null;
    private bool needsApply = false;

    void Start()
    {
        drawLayer = GetComponent<RawImage>();
        drawRect = GetComponent<RectTransform>();

        
        //reset the new texture
        ResetNewTexture();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //get correct mouse position
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawRect, Input.mousePosition, null, out localPoint);

            float x = localPoint.x + drawRect.rect.width / 2;
            float y = localPoint.y + drawRect.rect.height / 2;

            //draw
            if (x >= 0 && x < drawTexture.width && y >= 0 && y < drawTexture.height)
            {
                Vector2 currentPos = new Vector2(x, y);

                if (lastDrawPos.HasValue)
                    DrawLine(lastDrawPos.Value, currentPos);
                else
                    DrawAt((int)x, (int)y);

                lastDrawPos = currentPos;
            }
        }
        else
        {
            lastDrawPos = null;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveDrawing();
        }
    }

    void LateUpdate()
    {
        //only apply when need. 
        if (needsApply)
        {
            drawTexture.Apply();
            needsApply = false;
        }
    }

    void ResetNewTexture()
    {
        //reset the new texture
        int texWidth = Mathf.RoundToInt(drawRect.rect.width);
        int texHeight = Mathf.RoundToInt(drawRect.rect.height);

        drawTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;

        Color[] clearPixels = new Color[texWidth * texHeight];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = boardColor; //transparent

        drawTexture.SetPixels(clearPixels);

        drawLayer.texture = drawTexture;
        needsApply = true;
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        if (distance < brushSize * 0.3f) return;

        int steps = Mathf.CeilToInt(distance);
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawAt((int)point.x, (int)point.y);
        }
    }

    void DrawAt(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize)
                {
                    int px = Mathf.Clamp(x + i, 0, drawTexture.width - 1);
                    int py = Mathf.Clamp(y + j, 0, drawTexture.height - 1);
                    drawTexture.SetPixel(px, py, drawColor);
                }
            }
        }
        needsApply = true;
    }


    public void SaveDrawing()
    {
        Debug.Log("Interact called for: " + gameObject.name);

        // Handle Fungus block execution
        if (!string.IsNullOrEmpty(blockName) && flowchart != null)
        {
            if (!flowchart.HasExecutingBlocks())
            {
                Debug.Log("Executing block: " + blockName);
                flowchart.ExecuteBlock(blockName);
            }
        }
        else
        {
            Debug.LogWarning("No blockName provided or Flowchart not assigned!");
        }
    }

}