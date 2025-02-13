using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] Sprite two;
    [SerializeField] Sprite three;
    [SerializeField] Sprite four;
    public GameObject jumpscare;
    [SerializeField] GameObject gear;
    [SerializeField] AudioSource snd_plant;

    public static string flwrPref = "FlowerState"; // flower state saved name
    public static string gearState = "GearState"; // a bool
    int state = 0;
    public void Start()
    {
        if (PlayerPrefs.HasKey(flwrPref)) // if flower state already exists
        {
            SetState(PlayerPrefs.GetInt(flwrPref)); // load saved state
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(state == 3 && Input.GetKey(KeyCode.E))
        {
            PlayerPrefs.SetInt(gearState, 1); // gear state set
        }
    }

    public void SetState(int num)
    {
        state = num;
        if (num == 1) //if have interacted one time
        {
            snd_plant.Play();
            GetComponent<SpriteRenderer>().sprite = two;
            PlayerPrefs.SetInt(flwrPref, 1);
        }
        else if (num == 2)
        {
            snd_plant.Play();
            GetComponent<SpriteRenderer>().sprite = three;
            PlayerPrefs.SetInt(flwrPref, 2);
        }
        else if (num == 3)
        {
            snd_plant.Play();
            GetComponent<SpriteRenderer>().sprite = four;
            PlayerPrefs.SetInt(flwrPref, 3);

            //吐齿轮！
            if (!PlayerPrefs.HasKey(gearState)) // if gear has not yet been collected
            {
                gear.SetActive(true);
            }
        }
        Debug.Log("flower pref is " + PlayerPrefs.GetInt(flwrPref));
    }
}
