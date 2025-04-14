using Fungus;
using SKCell;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mirror_Puzzle : EInteractable
{
    public GameObject interactBt;
    public GameObject mirrorTalking;
    //public string exitBlock;
    bool inAnimation = false;
    public List<CanvasGroup> talkings;
    public Player_Ch1 player;
    //float timer;

    public Chapter1_Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < talkings.Count; i++)
        {
            talkings[i].alpha = 0f;
        }
        
    }

    private void Update()
    {
        
        // Only allow interaction if the player is in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!inAnimation)
            {
                Interact();
            } else
            {
                if (interactBt.activeSelf)
                {
                    inAnimation = false;
                    cam.ResetCamera();
                    mirrorTalking.SetActive(false);
                    player.canMove = true;
                }
            }
        }



    }
    public void ShowTalk()
    {
        mirrorTalking.SetActive(true);
        inAnimation = true;
        StartCoroutine(TextFadeInOut());
    }

    IEnumerator TextFadeInOut()
    {
        for (int i = 0; i < talkings.Count; i++)
        {
            StartCoroutine(FadeIn(talkings[i], 1f));
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
        interactBt.SetActive(true);
    }

    public IEnumerator FadeOut(CanvasGroup text,float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            text.alpha = 1 - (t / duration);
            yield return null;
        }
        text.alpha = 0f;
    }

    public IEnumerator FadeIn(CanvasGroup text, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            text.alpha = t / duration;
            yield return null;
        }
        text.alpha = 1f;
        StartCoroutine(FadeOut(text, Random.Range(2f, 3f), 1f));
    }
}
