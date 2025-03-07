using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicTransition : MonoBehaviour
{
    public CanvasGroup blackScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string musicSceneName;

    [SerializeField] private new GameObject animation;
    [SerializeField] private GameObject[] lighting;
    private Animator animator;
    private bool isAnimationComplete;

    private AsyncOperation preloadOperation;
    private bool hasTransitioned = false;
    private static bool scenePreloaded;

    void Awake()
    {
        if (animation != null)
        {
            animator = animation.GetComponent<Animator>();
            animator.enabled = false;
        }

        isAnimationComplete = false;
        hasTransitioned = false;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != musicSceneName && !scenePreloaded)
        {
            Debug.Log("Preloading scene in Start method");
            blackScreenCanvasGroup.alpha = 0;
            PreloadScene(musicSceneName);
            scenePreloaded = true;
        }
    }

    private void Update()
    {
        //Debug.Log("preloadOperation in Update: " + preloadOperation);
    }

    public void TransitionToScene()
    {
        if (hasTransitioned) return;
        hasTransitioned = true;
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        PlayAnimation();
        yield return null;
    }

    public void OnAnimationComplete()
    {
        isAnimationComplete = true;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        yield return new WaitForSeconds(0);
        float startAlpha = blackScreenCanvasGroup.alpha;
        float timeElapsed = 0;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            blackScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            yield return null;
        }

        blackScreenCanvasGroup.alpha = targetAlpha;
    }

    public void PreloadScene(string sceneName)
    {
        Debug.Log("Preloading scene: " + sceneName);
        preloadOperation = SceneManager.LoadSceneAsync(sceneName);
        if (preloadOperation == null)
        {
            Debug.LogError("Failed to preload scene: " + sceneName);
        }
        else
        {
            preloadOperation.allowSceneActivation = false;
            Debug.Log("Scene preloaded successfully");
        }
    }

    public void PlayAnimation()
    {
        if (lighting != null && lighting.Length > 0)
        {
            foreach (var light in lighting)
            {
                if (light != null)
                    light.SetActive(false);
            }
        }
        if (animation != null)
        {
            animator = animation.GetComponent<Animator>();
            animator.enabled = true;
            animation.SetActive(true);
            animator.Play("curtain1");
            animator.SetBool("Looping", false);
            StartCoroutine(WaitForAnimation());
        }
    }

    private IEnumerator WaitForAnimation()
    {
        yield return StartCoroutine(Fade(1));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        isAnimationComplete = true;
        Debug.Log("preloadOperation in WaitForAnimation before activation: " + preloadOperation);
        if (preloadOperation != null)
        {
            Debug.Log("Allowing scene activation in WaitForAnimation");
            preloadOperation.allowSceneActivation = true;
        }
        else
        {
            Debug.LogError("preloadOperation is null in WaitForAnimation");
        }
    }
}
