using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Rm1_mirror : Items
{
    [SerializeField] Sprite smashedMirror;
    GameObject key;
    string hammerName = "Hammer";
    string mirrorPref = "MirrorCracked";
    [SerializeField] AudioSource snd_break;

    [SerializeField] GameObject source;

    // for hammer animation
    Animator anim;
    float delay = 5f;
    int stage = 0; // 0 is uncracked, 1 is cracked

    protected override void Start()
    {
        base.Start();

        key = transform.GetChild(0).gameObject; // get first child, key
        key.SetActive(false); // disable

        anim = GetComponent<Animator>(); // get animator
        anim.enabled = false;

        if (PlayerPrefs.HasKey(mirrorPref)) // if mirror state already exists
        {
            stage = PlayerPrefs.GetInt(mirrorPref); // load saved state

            SmashedMirror();
        }
    }

    protected override void Interact()
    {
        // empty statement
    }


    private void SmashedMirror()
    {
        rend.sprite = smashedMirror;
        highlightedPic = smashedMirror;
        originalPic = smashedMirror;
        anim.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        if (stage == 0 && onObject && InventoryStorage.instance.selectedItemId == hammerName)
        {
            anim.enabled = true; // start animation
            InventoryStorage.instance.RemoveItem(hammerName);

            StartCoroutine(WaitToSwap(delay, smashedMirror));
            stage = 1; // next stage
            PlayerPrefs.SetInt(mirrorPref, stage);

            //sound effect
            snd_break.Play();

            //change music
            print("should CHANGE MUSIC");
            source.GetComponent<SoundManager>().PlaySong(3);
        }
    }

    IEnumerator WaitToSwap(float time, Sprite swap)
    {
        yield return new WaitForSeconds(time);
        rend.sprite = swap;

        key.SetActive(true); // enable key
        SmashedMirror();
    }
}
