using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite selectedSprite;
    public bool selected = false;
    public int num;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeSelect()
    {
        if (selected)
        {
            selected = false;
            image.sprite = normalSprite;
        }
        else
        {
            selected = true;
            image.sprite = selectedSprite;
        }
    }
}
