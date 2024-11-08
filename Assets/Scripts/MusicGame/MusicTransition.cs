using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicTransition : MonoBehaviour
{
    public CanvasGroup blackScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string musicSceneName = "MusicGameTest";
    
    [SerializeField] private GameObject animation;
    private Animator animator;
    private bool isAnimationComplete;
    
    private AsyncOperation preloadOperation;
    
    void Awake()
    {
        animator = animation.GetComponent<Animator>();
        isAnimationComplete = false;
    }
    
    private void Start()
    {
        blackScreenCanvasGroup.alpha = 0;
        PreloadScene(musicSceneName);
    }
    
    public void TransitionToScene()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        PlayAnimation();
        yield return new WaitUntil(() => isAnimationComplete);
        
        
        ChangeToMusicScene();
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

    public void ChangeToMusicScene()
    {
        preloadOperation.allowSceneActivation = true;
    }
    
    public void PreloadScene(string sceneName)
    {
        preloadOperation = SceneManager.LoadSceneAsync(sceneName);
        preloadOperation.allowSceneActivation = false;
    }
    
    public void PlayAnimation()
    {
        animation.SetActive(true);
        animator.Play("curtain1");
        animator.SetBool("Looping", false); 
        StartCoroutine(WaitForAnimation());
    }
    
    private IEnumerator WaitForAnimation()
    {
        isAnimationComplete = false;
        yield return StartCoroutine(Fade(1));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        isAnimationComplete = true;
    }
}

