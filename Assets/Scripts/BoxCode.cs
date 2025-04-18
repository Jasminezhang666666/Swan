using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxCode : MonoBehaviour
{
    int num = 9;

    public void Decrement()
    {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        num--;
        if (num < 0)
        {
            num = 9;
        }
        text.text = num.ToString();
    }
}
