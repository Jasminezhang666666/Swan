using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horrorSound : MonoBehaviour
{
    [SerializeField] AudioSource snd_horror;
    public int timer = 450;

    // Update is called once per frame
    void Update()
    {
        timer--;
        if (timer == 0)
        {
            snd_horror.Play();
        } 
    }
}
