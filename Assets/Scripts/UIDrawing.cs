using UnityEngine;
using UnityEngine.UI;

public class UIDrawing : MonoBehaviour
{
    public GameObject nameTag; 
    public RawImage drawLayer; 
    public Color drawColor = Color.black;
    public int brushSize = 5;

    private Texture2D drawTexture;
    private RectTransform drawRect;
    private Vector2? lastDrawPos = null;
    private bool needsApply = false;

    void Start()
    {
        drawRect = drawLayer.GetComponent<RectTransform>();
        int texWidth = Mathf.RoundToInt(drawRect.rect.width);
        int texHeight = Mathf.RoundToInt(drawRect.rect.height);

        // ????????????
        drawTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;

        // ????????
        Color[] clearPixels = new Color[texWidth * texHeight];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color(0, 0, 0, 0);
        drawTexture.SetPixels(clearPixels);
        drawTexture.Apply();

        // ??? RawImage ?
        drawLayer.texture = drawTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawRect, Input.mousePosition, null, out localPoint);

            float x = localPoint.x + drawRect.rect.width / 2;
            float y = localPoint.y + drawRect.rect.height / 2;

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
    }

    void LateUpdate()
    {
        if (needsApply)
        {
            drawTexture.Apply();
            needsApply = false;
        }
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
        Sprite drawnSprite = Sprite.Create(
            drawTexture,
            new Rect(0, 0, drawTexture.width, drawTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        // ? ???????? sprite
        SpriteRenderer sr = nameTag.GetComponent<SpriteRenderer>();
        sr.sprite = drawnSprite;
    }
}