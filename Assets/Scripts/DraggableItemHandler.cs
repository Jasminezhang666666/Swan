using UnityEngine;

public class DraggableItemHandler : MonoBehaviour
{
    public Transform wheel; // Reference to the Wheel transform
    public float proximityThreshold = 1f; // Distance within which the item will move to the wheel
    public float moveSpeed = 2f; // Speed at which the item will move to the wheel

    private bool isMovingToWheel = false; // Flag to control if the item is moving to the wheel
    private Vector3 targetPosition; // The target position to move towards

    void Update()
    {
        if (isMovingToWheel)
        {
            MoveToWheel();
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        isMovingToWheel = true; // Start moving towards the target position
    }

    private void MoveToWheel()
    {
        if (wheel == null)
        {
            isMovingToWheel = false;
            return;
        }

        // Move towards the wheel
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Check if close enough to become child
        if (Vector3.Distance(transform.position, targetPosition) < proximityThreshold)
        {
            transform.position = targetPosition; // Ensure exact position
            transform.SetParent(wheel); // Make wheel the parent
            Debug.Log($"{gameObject.name} has become a child of the wheel.");
            isMovingToWheel = false; // Stop moving once at the wheel
        }
    }
}
