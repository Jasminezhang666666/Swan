using UnityEngine;
using System.Collections;

public class HammerAndNail : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string stateName = "HammerAndNail";
    [SerializeField] private float stepSize = 0.25f;
    [SerializeField] private float clipLength = 1.15f;

    private float currentTime = 0f;
    private bool isStepping = false;

    void Start()
    {
        animator.enabled = true;
        animator.speed = 0f;
        animator.Play(stateName, 0, 0f);
        animator.Update(0);
    }

    /// <summary>
    /// Call this to advance the hammer animation by one step.
    /// </summary>
    public void StepHammer()
    {
        if (isStepping || currentTime >= clipLength)
            return;

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

        if (currentTime >= clipLength)
        {
            // --- right before disabling the hammer, mark the floor as fixed on both door scripts ---
            foreach (var door in FindObjectsOfType<E_Door_MultipleOutput>())
            {
                door.FixedFloor = true;
            }
            foreach (var door in FindObjectsOfType<TransparentDoor_Dressing02>())
            {
                door.FixedFloor = true;
            }

            // now disable the hammer
            enabled = false;
            gameObject.SetActive(false);
        }
    }
}
