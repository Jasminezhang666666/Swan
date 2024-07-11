using UnityEngine;

public class Puzzle4_wheel : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 previousMousePosition;
    private float angle;
    private float[] snapAngles = { 0, 90, 180, 270 };
    private float snapThreshold = 10f;
    private bool isSnapping = false;
    private float targetAngle;
    private float snapSpeed = 2f;

    void Start()
    {
        angle = 180f; // Set the initial rotation angle to 180 degrees
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            isSnapping = false;
            previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SnapToNearestAngle();
        }

        if (isDragging)
        {
            Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = currentMousePosition - (Vector2)transform.position;
            Vector2 previousDirection = previousMousePosition - (Vector2)transform.position;

            float angleDifference = Vector2.SignedAngle(previousDirection, direction);
            angle += angleDifference;

            transform.rotation = Quaternion.Euler(0, 0, angle);
            previousMousePosition = currentMousePosition;
        }

        if (isSnapping)
        {
            float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * snapSpeed);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.1f)
            {
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
                isSnapping = false;
            }
        }
    }

    private void SnapToNearestAngle()
    {
        float currentZAngle = transform.eulerAngles.z;
        foreach (float snapAngle in snapAngles)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(currentZAngle, snapAngle)) < snapThreshold)
            {
                targetAngle = snapAngle;
                isSnapping = true;
                break;
            }
        }
    }
}
