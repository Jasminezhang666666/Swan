using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] float waitTime;

    private void Awake()
    {
        if (!fade)
        {
            Debug.Log("Fade is unassigned, SHABI!!");
        }
    }

    private void Start()
    {
        fade.StartCoroutine("FadeIn"); // fade in
    }

    public void SwitchScene(string sceneName)
    {
        fade.StartCoroutine("FadeOut"); // fade to black
        StartCoroutine(Wait(waitTime, sceneName));
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator Wait(float duration, string sceneName)
    {
        yield return new WaitForSecondsRealtime(duration);
        SceneManager.LoadScene(sceneName);
    }
}
