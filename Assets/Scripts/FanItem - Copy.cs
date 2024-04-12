using UnityEngine;
using UnityEngine.UI;

public class FanItem : MonoBehaviour
{
    public Image itemImage;  // The UI Image component that displays the item's sprite

    // This method updates the UI image to display the given sprite
    public void SetItemSprite(Sprite newSprite)
    {
        if (itemImage != null)
        {
            itemImage.sprite = newSprite;
            itemImage.enabled = (newSprite != null); // Enable or disable based on if there's a sprite to display
        }
    }

    // Optionally, add more methods to handle other item-specific UI behaviors
}
