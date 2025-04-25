using UnityEngine;

public class MainPuzzle1_itemFinal : MainPuzzle1_item
{
    [Header("Room Swap Objects")]
    public GameObject dressingRoom;
    public GameObject puzzle1;

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

        // Disable collider so it can't be clicked again
        if (TryGetComponent<Collider>(out Collider col)) col.enabled = false;

        // Hide the item's visual renderer
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr)) sr.enabled = false;
        else if (TryGetComponent<MeshRenderer>(out MeshRenderer mr)) mr.enabled = false;

        // Animate into inventory from its current world position
        NewItem newItem = GetComponent<NewItem>();
        if (newItem != null)
        {
            newItem.GoToInventoryFromWorld(transform.position, () =>
            {
                dressingRoom?.SetActive(true);
                puzzle1?.SetActive(false);
            });
        }
        else
        {
            Debug.LogWarning($"NewItem missing on {name}");
        }
    }
}
