using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle2_Hammer : DraggableItem
{
    public static bool HammerDragging = false;

    private void Start()
    {
        HammerDragging = true; // Automatically start dragging when created
    }

    private void Update()
    {
        if (HammerDragging)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the item stays on the same plane
        transform.position = mousePosition;
    }

    private void OnMouseDown()
    {
        HammerDragging = true;
    }

    private void OnMouseUp()
    {
        HammerDragging = true; // Keep dragging true to follow the mouse
    }
}
