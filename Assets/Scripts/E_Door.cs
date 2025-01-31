using UnityEngine;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class E_Door : EInteractable
{
    [SerializeField] private Sprite openDoorSprite; // Sprite for the open door
    [SerializeField] private string sceneName; // Scene name for teleporting
    private Sprite originalSprite; // Store the original sprite for reverting
    private SpriteRenderer spriteRenderer;
    public AK.Wwise.Event Door_Open;

    private void Start()
    {
        // Cache the sprite renderer and the original sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);


        // Only change sprite if the current chapter is not Chapter1
        if (other.CompareTag("Player") && !ChapterManager.Instance.IsChapter(ChapterManager.Chapter.Chapter1))
        {

            // Change to the open door sprite when player enters the collision
            spriteRenderer.sprite = openDoorSprite;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);


        // Only revert sprite if the current chapter is not Chapter1
        if (other.CompareTag("Player") && !ChapterManager.Instance.IsChapter(ChapterManager.Chapter.Chapter1))
        {


            // Revert to the original sprite when player leaves the collision
            spriteRenderer.sprite = originalSprite;
        }
    }

    public override void Interact()
    {
        // Make sure we only do something if the player is in range
        if (!isPlayerInRange)
        {
            return;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            Door_Open.Post(this.gameObject);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            //Fungus
            base.Interact();
            // Optionally add a debug log: 
            Debug.Log("Scene name is empty. Showing Fungus text only.");
        }
    }

}
