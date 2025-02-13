using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonActions : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] GameObject music;
    [SerializeField] AudioSource snd_mouseOver;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("StartAnim");
        Destroy(music);
    }

    public void DisplayNames()
    {
        SceneManager.LoadScene("Open_credits");
    }

    public void QuitGame()
    {
        Application.Quit();
        
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        snd_mouseOver.Play();
    }

}
