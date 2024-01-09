using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;

public class Fade : MonoBehaviour
{
    [SerializeField] float step = 0.1f;
    [SerializeField] float delay = 0.1f;
    
    Image black;
    float fullColor = 255f;

    Color transparent = new Color(0f, 0f, 0f, 0f); // initial color: black and transparent
    Color opaque = new Color(0f, 0f, 0f, 255f); // target color: black and opaque

    private void Awake()
    {
        black = GetComponent<Image>(); // image component
    }

    public IEnumerator FadeOut() // fade to black
    {
        black.color = transparent; // set image to transparent
        float opacity = black.color.a / fullColor;

        while (opacity < 1f)
        {
            opacity += step;
            black.color = new Color(black.color.r, black.color.g, black.color.b, opacity);
            //Debug.Log(black.color);
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator FadeIn() // fade from black
    {
        black.color = opaque; // set image to black
        float opacity = black.color.a / fullColor;

        while (opacity > 0f)
        {
            opacity -= step;
            black.color = new Color(black.color.r, black.color.g, black.color.b, opacity);
            //Debug.Log(black.color);
            yield return new WaitForSeconds(delay);
        }
    }
}
