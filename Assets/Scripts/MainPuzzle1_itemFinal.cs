using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPuzzle1_itemFinal : MainPuzzle1_item
{
    public Vector3 targetScale = new Vector3(2f, 2f, 2f); // Target scale to grow to
    public float moveToCenterDuration = 2f; // Duration in seconds to move to the center
    public float scaleDuration = 2f; // Duration in seconds to scale up
    public Transform designatedLocation; // Serialize field for designated location
    private string nextSceneName = "Inventory"; // Name of the next scene to load

    protected override void OnMouseUp()
    {
        base.OnMouseUp(); // Call the base method

        // Start the movement and scaling only if the item was dropped
        StartCoroutine(MoveAndScale());
    }

    IEnumerator MoveAndScale()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(0, 0, startPosition.z); // Center position
        Vector3 startScale = transform.localScale;

        float time = 0;

        // Move to the center and grow in scale
        while (time < moveToCenterDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / moveToCenterDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / moveToCenterDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the position is exactly the center and scale is at target size after moving
        transform.position = targetPosition;
        transform.localScale = targetScale;

        // Wait a moment before shrinking and moving to the designated location
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        // Reset scale and move to designated location
        time = 0;
        while (time < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, startScale, time / scaleDuration);
            transform.position = Vector3.Lerp(targetPosition, designatedLocation.position, time / scaleDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the scale is exactly the original size and position is exactly at the designated location
        transform.localScale = startScale;
        transform.position = designatedLocation.position;

        // Wait a moment before changing the scene
        yield return new WaitForSeconds(1); // Adjust the delay as needed

        // Change the scene
        SceneManager.LoadScene(nextSceneName);
    }
}
