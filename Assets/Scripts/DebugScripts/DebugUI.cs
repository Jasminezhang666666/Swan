using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DebugUI : MonoBehaviour
{
    private GameObject lastHoveredUI = null; // To avoid redundant logs

    void Update()
    {
        // Get the first UI element under the cursor
        GameObject hoveredUI = GetFirstUIElementUnderCursor();

        // Only log when the hovered UI element changes
        if (hoveredUI != lastHoveredUI)
        {
            lastHoveredUI = hoveredUI;
            if (hoveredUI != null)
            {
                Debug.Log("Hovering over: " + hoveredUI.name);
            }
            else
            {
                Debug.Log("Not hovering over any UI.");
            }
        }
    }

    private GameObject GetFirstUIElementUnderCursor()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }
}
