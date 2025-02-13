using UnityEngine;

public class Jane : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 2f; // Speed at which Jane moves.
    [SerializeField] private Transform[] locations;   // Array of locations Jane will move to.
    private int currentLocationIndex = 0;             // Index to track the current location.
    private bool isMoving = false;                    // Check if Jane is currently moving.
    private Vector3 originalScale;                    // Store the original scale of Jane.

    private void Start()
    {
        // Store Jane's original local scale.
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Check for space key press and if Jane is not already moving.
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && currentLocationIndex < locations.Length)
        {
            isMoving = true;
        }

        // If Jane is moving, move towards the target location.
        if (isMoving)
        {
            MoveToLocation(locations[currentLocationIndex].position);
        }
    }

    private void MoveToLocation(Vector3 targetPosition)
    {
        // Move Jane towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);

        // Flip the sprite based on the direction of movement, maintaining the original scale.
        if (targetPosition.x < transform.position.x)
        {
            // Face left if the target is to the left.
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (targetPosition.x > transform.position.x)
        {
            // Face right if the target is to the right.
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Check if Jane has reached the target position.
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            currentLocationIndex++; // Move to the next location for the next input.
        }
    }
}
