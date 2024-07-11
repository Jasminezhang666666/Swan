using UnityEngine;

public class puzzle4_button : MonoBehaviour
{
    public Sprite newSprite; // The sprite to switch to on hover
    public GameObject wheel; // The reference to the wheel object
    private Sprite originalSprite; // The original sprite to switch back to when not hovering
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool wheelActivated = false; // Track the state of the wheel

    void Start()
    {
        // Get the SpriteRenderer component attached to this object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original sprite
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }

        // Ensure the wheel is initially not activated
        if (wheel != null)
        {
            wheel.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        // Change to the new sprite when the mouse enters the object's collider
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    void OnMouseExit()
    {
        // Change back to the original sprite when the mouse exits the object's collider
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
    }

    void OnMouseDown()
    {
        // Toggle the wheel activation when the button is clicked
        if (wheel != null)
        {
            wheelActivated = !wheelActivated;
            wheel.SetActive(wheelActivated);
        }
    }
}