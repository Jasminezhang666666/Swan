using Fungus;
using SKCell;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MirrorAndBox_Puzzle : EInteractable
{
    public ScriptableRendererFeature screenEffect;

    [Header("Box Settings")]
    public GameObject boxInScene;
    public GameObject boxCode;
    public List<string> CodeList;
    public GameObject box;
    public GameObject boxOpenObj;
    bool boxOpen = false;

    public GameObject interactBt;
    public GameObject mirrorTalking;
    //public string exitBlock;
    bool inAnimation = false;
    public List<CanvasGroup> talkings;

    public CanvasGroup Transition;
    public GameObject laterPlayer;
    public GameObject fallPlayer;

    public AK.Wwise.Event doorSlam;

    //public Player_Ch1 player;
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
                    player.enabled = true;
                }
            }
        }

    }

    public void CheckBoxCode()
    {
        bool result = true;
        for (int i = 0; i < boxCode.transform.childCount; i++)
        {
            TextMeshProUGUI text = boxCode.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            if (text.text != CodeList[i])
            {
                result = false;
                break;
            }
        }
        if (result)
        {
            boxOpen = true;
            box.SetActive(false);
            boxOpenObj.SetActive(true);
            boxCode.SetActive(false);
            boxInScene.GetComponent<EInteractable>().enabled = false;
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

    public void GoToNextScene()
    {
        StartCoroutine(FadeInTransition(Transition, 2f, () =>
        {
            TurnOnOffScreenEffect(false);
            SceneManager.LoadScene("Rm_DanceStudio04");
        }));
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeInTransition(Transition, 2f, () =>
        {
            laterPlayer.SetActive(true);
            StartCoroutine(FadeOut(Transition, 0f, 2f));
            fallPlayer.SetActive(false);
            doorSlam.Post(gameObject);
            TurnOnOffScreenEffect(true);
        }));
    }

    IEnumerator FadeInTransition(CanvasGroup text, float duration, System.Action onComplete)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            text.alpha = t / duration;
            yield return null;
        }
        text.alpha = 1f;
        onComplete?.Invoke();
    }

    /// <summary>
    /// turn on or off screen effect
    /// </summary>
    /// <param name="enabled"></param>
    public void TurnOnOffScreenEffect(bool enabled)
    {
        screenEffect.SetActive(enabled);
    }


    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && screenEffect != null)
        {
            screenEffect.SetActive(false);
            Debug.Log("aaaaaaaa");
            UnityEditor.EditorUtility.SetDirty(screenEffect);
        }
#endif
    }
}
