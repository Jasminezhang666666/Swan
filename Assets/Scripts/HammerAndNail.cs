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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isStepping && currentTime < clipLength)
        {
            float nextTargetTime = Mathf.Min(currentTime + stepSize, clipLength);
            StartCoroutine(PlayToTime(nextTargetTime));
        }
    }

    IEnumerator PlayToTime(float targetTime)
    {
        isStepping = true;
        float timePlayed = 0f;
        float playDuration = targetTime - currentTime;

        animator.speed = 1f;
        animator.Play(stateName, 0, currentTime / clipLength);

        while (timePlayed < playDuration)
        {
            timePlayed += Time.deltaTime;
            yield return null;
        }

        currentTime = targetTime;
        animator.speed = 0f;
        
        float normalized = Mathf.Min(currentTime / clipLength, clipLength - 0.01f);
        animator.Play(stateName, 0, normalized);
        animator.Update(0);

        isStepping = false;
    
        //finish the animation
        if (currentTime >= clipLength)
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
        }
    }
}