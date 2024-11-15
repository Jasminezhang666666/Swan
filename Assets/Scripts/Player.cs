using UnityEngine;
using Fungus;
using AK.Wwise;

public class Player : MonoBehaviour
{
    public float speed = 4f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private float currentSpeed;

    private Animator _animator;
    private bool isFacingRight = true; // Track the current facing direction

    public float xMaxBound = 9f, xMinBound = -9f;

    private SpriteRenderer idleSpr;
    private GameObject childObject; // Reference to the child object

    public AK.Wwise.Event onFootstep;
    private uint onFootstep_playingID;


    // Reference to the Fungus Flowchart
    public Flowchart flowchart;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smooths movement
        currentSpeed = 0f;

        _animator = GetComponent<Animator>();
        idleSpr = GetComponent<SpriteRenderer>();

        // Assume the first child is the one to control; adjust if needed
        if (transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
        }
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
            idleSpr.enabled = true; // Show idle sprite when Fungus is playing
        }

        // Update Animator and sprite visibility based on movement state
        bool isWalking = canMove && Mathf.Abs(currentSpeed) > Mathf.Epsilon && !isFungusExecuting;
        _animator.SetBool("isMoving", isWalking);
        idleSpr.enabled = !isWalking; // Show idle sprite only when not moving

        // Enable child object only when walking
        if (childObject != null)
        {
            childObject.SetActive(isWalking);
        }

        // Play or stop the walking sound based on movement state // snd_walk
        if (isWalking)
        {
            if (onFootstep_playingID == 0)
            {
                onFootstep_playingID = onFootstep.Post(this.gameObject);
                Debug.Log("Footstep started");
            }
        }
        else
        {
            if (onFootstep_playingID != 0)
            {
                AkSoundEngine.StopPlayingID(onFootstep_playingID);
                onFootstep_playingID = 0; 
                Debug.Log("Footstep stopped");
            }
        }
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
