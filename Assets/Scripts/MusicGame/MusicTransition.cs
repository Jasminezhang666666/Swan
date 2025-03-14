using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicTransition : MonoBehaviour
{
    public CanvasGroup blackScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string musicSceneName;
    
    [SerializeField] private GameObject transitionAnimation;
    [SerializeField] private GameObject[] lighting;
    
    private Animator animator;
    private bool isAnimationComplete;
    private AsyncOperation preloadOperation;
    private bool hasTransitioned = false;
    private static bool scenePreloaded;

    void Awake()
    {
        // Check if transitionAnimation is assigned and has an Animator component.
        if (transitionAnimation != null)
        {
            animator = transitionAnimation.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator component found on the transitionAnimation GameObject.");
            }
            else
            {
                animator.enabled = false;
            }
        }

        // Ensure CanvasGroup is assigned.
        if (blackScreenCanvasGroup == null)
        {
            Debug.LogError("blackScreenCanvasGroup is not assigned!");
        }

        isAnimationComplete = false;
        hasTransitioned = false;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != musicSceneName && !scenePreloaded)
        {
            blackScreenCanvasGroup.alpha = 0;
            PreloadScene(musicSceneName);
            scenePreloaded = true;
        }
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
        // Brief wait before starting the fade (if needed)
        yield return new WaitForSeconds(0);
        float startAlpha = blackScreenCanvasGroup.alpha;
        float timeElapsed = 0f;

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
        preloadOperation = SceneManager.LoadSceneAsync(sceneName);
        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = false;
        }
    }

    public void PlayAnimation()
    {
        // Disable lighting GameObjects if they are set.
        if (lighting != null && lighting.Length > 0)
        {
            foreach (var light in lighting)
            {
                if (light != null)
                    light.SetActive(false);
            }
        }

        if (transitionAnimation != null)
        {
            // Activate the transition animation object before using its components.
            transitionAnimation.SetActive(true);

            if (animator != null)
            {
                animator.enabled = true;
                // Check if a runtimeAnimatorController is assigned.
                if (animator.runtimeAnimatorController == null)
                {
                    return;
                }

                // Check if the specified animation state exists.
                if (animator.HasState(0, Animator.StringToHash("TransitionCurtain_anim")))
                {
                    animator.Play("TransitionCurtain_anim");
                    StartCoroutine(WaitForAnimation());
                }
            }
        }
    }

    private IEnumerator WaitForAnimation()
    {
        // Start fade-out to full opacity.
        yield return StartCoroutine(Fade(1));

        // Wait until the animation has played completely.
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        isAnimationComplete = true;

        // Activate the preloaded scene once the animation is complete.
        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(musicSceneName);
        }
    }
}
