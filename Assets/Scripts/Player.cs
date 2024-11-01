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
    private Vector3 originalScale; // Store the original scale

    [SerializeField] AudioSource snd_walk;

    public float xMaxBound = 9f, xMinBound = -9f;

    private SpriteRenderer idleSpr;
    private GameObject walkingAnim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = 0f;
        previousKey = KeyCode.None;

        _animator = GetComponent<Animator>();

        // Store the initial local scale
        originalScale = transform.localScale;

        /*
        if (PlayerPrefs.HasKey(Door.doorPref)) // if there is a target location
        {
            float location = PlayerPrefs.GetFloat(Door.doorPref);
            transform.position = new Vector2(location, transform.position.y); // set to target location
            Debug.Log("Door location = " + location);
        }
        */

        idleSpr = GetComponent<SpriteRenderer>();
        walkingAnim = transform.GetChild(0).gameObject;
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
                _animator.SetBool("isMoving", true);
                idleSpr.enabled = false;
                walkingAnim.SetActive(true);

                currentSpeed = -speed;
                previousKey = KeyCode.A;
            }
            else if (rb.position.x <= xMaxBound && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) // check that left bound not exceeded
            {
                if (previousKey != KeyCode.D)
                {
                    currentSpeed = 0;
                }

                _animator.SetBool("isMoving", true);
                idleSpr.enabled = false;
                walkingAnim.SetActive(true);

                currentSpeed = speed;
                previousKey = KeyCode.D;
            }
            else
            {
                currentSpeed = 0;
                previousKey = KeyCode.None;

                _animator.SetBool("isMoving", false);
                idleSpr.enabled = true;
                walkingAnim.SetActive(false);
            }
        }
        else // other cases
        {
            currentSpeed = 0;
            previousKey = KeyCode.None;
        }

        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        //if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        //{
        //    _animator.SetBool("isMoving", true);
        //    //if (!snd_walk.isPlaying)
        //    //{
        //    //    snd_walk.Play();
        //    //}
        //}
        //else
        //{
        //    _animator.SetBool("isMoving", false);
        //    //snd_walk.Stop();
        //}

        // flip character given movement direction
        if (rb.velocity.x > Mathf.Epsilon) // moving right
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // normal scale
        }
        else if (rb.velocity.x < -Mathf.Epsilon) // moving left
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z); // flip scale on X-axis
        }
    }
}
