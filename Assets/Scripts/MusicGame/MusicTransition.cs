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
    private bool hasTransitioned = false;

    void Awake()
    {
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

        if (blackScreenCanvasGroup == null)
        {
            Debug.LogError("blackScreenCanvasGroup is not assigned!");
        }
    }

    private void Start()
    {
        if (blackScreenCanvasGroup != null)
        {
            blackScreenCanvasGroup.alpha = 0;
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

    private IEnumerator Fade(float targetAlpha)
    {
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

        if (transitionAnimation != null)
        {
            transitionAnimation.SetActive(true);

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.enabled = true;

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
        yield return StartCoroutine(Fade(1));

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        SceneManager.LoadScene(musicSceneName);
    }
}
