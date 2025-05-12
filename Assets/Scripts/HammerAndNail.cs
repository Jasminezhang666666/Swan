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

    private float currentTime = 0f;
    private bool isStepping = false;
    private int useCount = 0;
    private const int maxUses = 3;

    void Start()
    {
        // hide both visuals initially
        if (hammerChild != null) hammerChild.SetActive(false);
        if (longNailChild != null) longNailChild.SetActive(false);

        animator.enabled = true;
        animator.speed = 0f;
        animator.Play(stateName, 0, 0f);
        animator.Update(0);
    }

    // public methods to enable each child separately
    public void EnableHammerChild()
    {
        if (hammerChild != null)
            hammerChild.SetActive(true);
    }

    public void EnableLongNailChild()
    {
        if (longNailChild != null)
            longNailChild.SetActive(true);
    }

    /// <summary>
    /// Call this to advance the hammer animation by one step.
    /// Only works once both children are active and uses < maxUses.
    /// </summary>
    public void StepHammer()
    {
        // only allow if:
        //  - not already stepping
        //  - haven't hit the clip length
        //  - haven't used up all allowed uses
        //  - both children are active
        if (isStepping ||
            currentTime >= clipLength ||
            useCount >= maxUses ||
            hammerChild == null || !hammerChild.activeSelf ||
            longNailChild == null || !longNailChild.activeSelf)
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
        float playDuration = targetTime - currentTime;

        animator.speed = 1f;
        animator.Play(stateName, 0, currentTime / clipLength);

        float elapsed = 0f;
        while (elapsed < playDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentTime = targetTime;
        animator.speed = 0f;

        float normalized = Mathf.Min(currentTime / clipLength, clipLength - 0.01f);
        animator.Play(stateName, 0, normalized);
        animator.Update(0);

        isStepping = false;

        // once we've used it maxUses times, disable interactivity & visuals
        if (useCount >= maxUses)
        {
            // mark floors fixed
            foreach (var door in FindObjectsOfType<E_Door_MultipleOutput>())
                door.FixedFloor = true;
            foreach (var door in FindObjectsOfType<TransparentDoor_Dressing02>())
                door.FixedFloor = true;

            // disable our collider so EInteractable fires OnTriggerExit2D
            var col2d = GetComponent<Collider2D>();
            if (col2d != null)
                col2d.enabled = false;

            // hide both child visuals
            hammerChild?.SetActive(false);
            longNailChild?.SetActive(false);

            // (optional) stop this script
            // enabled = false;
        }
    }
}
