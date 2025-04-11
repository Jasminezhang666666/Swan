using Fungus;
using SKCell;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mirror_Puzzle : MonoBehaviour
{
    public List<TextMeshProUGUI> talkings;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTalk()
    {
        gameObject.SetActive(true);
    }

    void TextFadeInOut()
    {
        if (talkings.Count > 0)
        {
            TextMeshProUGUI text = talkings[0];
            if (timer <= 0)
            {
                /*
                AudioManager.instance.PlaySound("emotion appear");
                GameManager.instance.currentMemory1[index].wordRect.anchoredPosition = TextOverlapCheck(index);
                GameManager.instance.currentMemory1[index].wordAni.SetTrigger("Play");
                Word word = GameManager.instance.currentMemory1[index];
                GameManager.instance.currentMemory1.RemoveAt(index);
                StartCoroutine(PutWordBack(word));
                timer = UnityEngine.Random.Range(0.5f, 1.5f);
                */
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
