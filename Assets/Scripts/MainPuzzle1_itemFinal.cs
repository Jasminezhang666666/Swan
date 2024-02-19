using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Make sure to include this for scene management

public class MainPuzzle1_itemFinal : MainPuzzle1_item
{
    public Vector3 targetScale = new Vector3(2f, 2f, 2f); // Target scale to grow to
    public float moveToCenterDuration = 2f; // Duration in seconds to move to the center
    public float scaleDuration = 2f; // Duration in seconds to scale up
    public string nextSceneName = "NextScene"; // Name of the next scene to load

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        isDragging = false; // Stop dragging
        myCollider.enabled = false; // Disable collider to stop interacting

        if (myRenderer != null)
        {
            myRenderer.sortingOrder = 1000; // Set this value high enough to be on top of all other objects
        }

        StartCoroutine(MoveAndScale());
    }


    IEnumerator MoveAndScale()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0, 0, startPosition.z); // Assuming the center is at (0,0)
        Vector3 startScale = transform.localScale;
        // Ensure targetScale is greater than startScale for all components to scale up
        targetScale = startScale * 2; // Example: Scale up by 2 times the original size

        float time = 0;

        while (time < moveToCenterDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / moveToCenterDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the position is exactly the center after moving
        transform.position = targetPosition;

        time = 0; // Reset time for scaling

        while (time < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / scaleDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the scale is exactly the target scale after scaling
        transform.localScale = targetScale;

        // Wait a few seconds before changing the scene
        yield return new WaitForSeconds(2); // Adjust the delay as needed

        // Change the scene
        SceneManager.LoadScene(nextSceneName);
    }

}
