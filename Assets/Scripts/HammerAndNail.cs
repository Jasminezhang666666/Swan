using UnityEngine;
using System.Collections;

public class HammerAndNail : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName = "HammerAndNail";
    [SerializeField] private float stepSize = 0.25f;
    [SerializeField] private float clipLength = 1.15f;

    [Header("Child Objects")]
    [SerializeField] private GameObject hammerChild;
    [SerializeField] private GameObject longNailChild;

    [Header("Invisible Wall")]
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;

    private float currentTime = 0f;
    private bool isStepping = false;
    private int useCount = 0;
    private const int maxUses = 3;

    void Start()
    {
        // hide both visuals initially
        if (hammerChild != null)
            hammerChild.SetActive(false);
        if (longNailChild != null)
            longNailChild.SetActive(false);

        // initialize animator at start frame and pause
        animator.enabled = true;
        animator.speed = 0f;
        animator.Play(stateName, 0, 0f);
        animator.Update(0);
    }

    // public getters so E_Hammer can check
    public bool HammerEnabled => hammerChild != null && hammerChild.activeSelf;
    public bool NailEnabled => longNailChild != null && longNailChild.activeSelf;

    // methods to flip them on
    public void EnableHammerChild()
    {
        hammerChild?.SetActive(true);
    }

    public void EnableLongNailChild()
    {
        longNailChild?.SetActive(true);
    }

    public void StepHammer()
    {
        // guard: only if both are on, under max uses, etc.
        if (isStepping
            || currentTime >= clipLength
            || useCount >= maxUses
            || !HammerEnabled
            || !NailEnabled)
        {
            return;
        }

        useCount++;
        float nextTarget = Mathf.Min(currentTime + stepSize, clipLength);
        StartCoroutine(PlayToTime(nextTarget));
    }

    private IEnumerator PlayToTime(float targetTime)
    {
        isStepping = true;
        animator.speed = 1f;
        animator.Play(stateName, 0, currentTime / clipLength);

        float duration = targetTime - currentTime;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentTime = targetTime;
        animator.speed = 0f;
        float normalized = (currentTime >= clipLength ? 1f : currentTime / clipLength);
        animator.Play(stateName, 0, normalized);
        animator.Update(0);

        isStepping = false;

        // on last use, disable collider & visuals and freeze on final frame
        if (useCount >= maxUses)
        {
            // mark floors fixed
            foreach (var door in FindObjectsOfType<E_Door_MultipleOutput>())
                door.FixedFloor = true;
            foreach (var door in FindObjectsOfType<TransparentDoor_Dressing02>())
                door.FixedFloor = true;

            // disable interaction collider
            Collider2D col2d = GetComponent<Collider2D>();
            if (col2d != null)
                col2d.enabled = false;

            // hide hammer and lower nail visuals
            hammerChild?.SetActive(false);
            longNailChild?.SetActive(false);

            // freeze animator at last frame
            animator.Play(stateName, 0, 1f);
            animator.Update(0);
            animator.enabled = false;

            //destroy both 空气墙
            Destroy(wall1);
            Destroy(wall2);
        }
    }
}
