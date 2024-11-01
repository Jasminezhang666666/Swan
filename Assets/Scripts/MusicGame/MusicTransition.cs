using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicTransition : MonoBehaviour
{
    public CanvasGroup blackScreenCanvasGroup; 
    [SerializeField]private float fadeDuration = 1f;
    [SerializeField] private string musicSceneName = "MusicGameTest";
    
    private AsyncOperation preloadOperation;
    
    private void Start()
    {
        blackScreenCanvasGroup.alpha = 0;
    }
    
    

    public void TransitionToScene()
    {
        PreloadScene(musicSceneName);
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        yield return StartCoroutine(Fade(1));
        ChangeToMusicScene();
    }

    private IEnumerator Fade(float targetAlpha)
    {
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
        SceneManager.LoadScene(musicSceneName);
    }
    
    public void PreloadScene(string sceneName)
    {
        preloadOperation = SceneManager.LoadSceneAsync(sceneName);
        preloadOperation.allowSceneActivation = false; 
    }
}
