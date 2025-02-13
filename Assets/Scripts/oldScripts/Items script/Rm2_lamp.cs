using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm2_lamp : Items
{
    int timer = 0;
    int lightOnTime = 80;
    public int haveInteracted = 0;
    [SerializeField] Sprite lightOn;
    [SerializeField] GameObject flower;

    [SerializeField] AudioSource snd_lamp;
    [SerializeField] AudioSource snd_horror;

    protected override void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey(Flower.flwrPref)) // if flower state already exists
        {
            haveInteracted = PlayerPrefs.GetInt(Flower.flwrPref); // load saved state
        }

        HasKey(Flower.flwrPref);

        //interctDia.Add("A floor lamp with a string that you could pull.");
    }

    public void HasKey(string KeyName)
    {
        if (PlayerPrefs.HasKey(KeyName))
        {
            Debug.Log("The key " + KeyName + " exists, with value " + PlayerPrefs.GetInt(Flower.flwrPref));
        }
        else
            Debug.Log("The key " + KeyName + " does not exist");
    }

    protected override void Interact()
    {
        isInteracting = true;

        snd_lamp.Play(); //Sound EFFECT

        timer = lightOnTime;
        //turn the light on
        GetComponent<SpriteRenderer>().sprite = lightOn;
        InventoryStorage.instance.player.canMove = false;


        haveInteracted++;
        flower.GetComponent<Flower>().SetState(haveInteracted);

        // trigger jumpscare
        if (haveInteracted == 3)
        {
            flower.GetComponent<Flower>().jumpscare.SetActive(true);
            snd_horror.Play();
            Debug.Log("jumpscare");
        }
    }

    protected void FixedUpdate()
    {
        if (isInteracting) {

            if (timer > 0)
            {
                //print(timer);
                timer--;
            } else
            {
                //turn the light off again
                GetComponent<SpriteRenderer>().sprite = originalPic;

                isInteracting = false;
                InventoryStorage.instance.player.canMove = true;
                if(haveInteracted == 3)
                {
                    flower.GetComponent<Flower>().jumpscare.SetActive(false);
                }
            }
        }
    }
}
