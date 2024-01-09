using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 4f;
    public float maxSpeed = 7f;
    public bool canMove = true;

    private Rigidbody2D rb;
    private float currentSpeed;
    private KeyCode previousKey;

    private Animator _animator;

    [SerializeField] AudioSource snd_walk;

    public float xMaxBound = 9f, xMinBound = -9f;
    SpriteRenderer rend; // sprite renderer for flipping

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = 0f;
        previousKey = KeyCode.None;

        rend = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();


        if (PlayerPrefs.HasKey(Door.doorPref)) // if there is a target location
        {
            float location = PlayerPrefs.GetFloat(Door.doorPref);
            transform.position = new Vector2(location, transform.position.y); // set to target location
            Debug.Log("Door location = " + location);
        }
    }

    private void Update()
    {
        if (canMove)
        {
            if (rb.position.x >= xMinBound && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))) // check that right bound not exceeded
            {
                if (previousKey != KeyCode.A)
                {
                    currentSpeed = 0;
                }
                // currentSpeed = Mathf.MoveTowards(currentSpeed, -maxSpeed, speed * Time.deltaTime);
                currentSpeed = -speed;
                previousKey = KeyCode.A;
            }
            else if (rb.position.x <= xMaxBound && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) // check that left bound not exceeded
            {
                if (previousKey != KeyCode.D)
                {
                    currentSpeed = 0;
                }
                // currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, speed * Time.deltaTime);
                currentSpeed = speed;
                previousKey = KeyCode.D;
            }
            else
            {
                currentSpeed = 0;
                previousKey = KeyCode.None;
            }
        } else // other cases
        {
            currentSpeed = 0;
            previousKey = KeyCode.None;

        }
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);


        if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        {
            _animator.SetBool("isMoving", true);
            if(!snd_walk.isPlaying)
            {
                snd_walk.Play();
            }

        } else
        {
            _animator.SetBool("isMoving", false);
            snd_walk.Stop();
        }

        // flip character given movement direction
        if (rb.velocity.x > Mathf.Epsilon) // positive velocity
        {
            rend.flipX = false;
        } else if (rb.velocity.x < -Mathf.Epsilon) // negative velocity
        {
            rend.flipX = true;
        }
    }
  
}
