using UnityEngine;
using Fungus;

public class Player : MonoBehaviour
{
    public float speed = 4f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private float currentSpeed;

    private Animator _animator;
    private bool isFacingRight = true; // Track the current facing direction

    [SerializeField] AudioSource snd_walk;
    public float xMaxBound = 9f, xMinBound = -9f;

    private SpriteRenderer idleSpr;

    // Reference to the Fungus Flowchart
    public Flowchart flowchart;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smooths movement
        currentSpeed = 0f;

        _animator = GetComponent<Animator>();
        idleSpr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Check if Fungus is playing a block
        bool isFungusExecuting = flowchart != null && flowchart.HasExecutingBlocks();

        if (canMove && !isFungusExecuting)
        {
            HandleMovement();
        }
        else
        {
            currentSpeed = 0; // Stop horizontal movement
            rb.velocity = new Vector2(0, rb.velocity.y); // Ensure no horizontal movement

            // Stop animation when Fungus is playing
            _animator.SetBool("isMoving", false);
        }

        // Update Animator based on movement state
        _animator.SetBool("isMoving", canMove && Mathf.Abs(currentSpeed) > Mathf.Epsilon && !isFungusExecuting);
    }

    private void FixedUpdate()
    {
        // Only apply velocity if the player can move and Fungus is not executing
        if (canMove && !(flowchart != null && flowchart.HasExecutingBlocks()))
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Ensure velocity is reset if canMove is false
        }
    }

    private void HandleMovement()
    {
        bool movingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool movingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (movingLeft && rb.position.x > xMinBound)
        {
            currentSpeed = -speed;
            if (isFacingRight)
            {
                Flip();
            }
        }
        else if (movingRight && rb.position.x < xMaxBound)
        {
            currentSpeed = speed;
            if (!isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            currentSpeed = 0;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
