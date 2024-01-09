using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rm5_mirror : MonoBehaviour
{
    [SerializeField] GameObject swan;
    [SerializeField] float delay;
    SpriteRenderer rend;
    Animator anim;
    AudioSource source;

    bool onObject = false;
    bool interactable = true;

    string playerPrefKey = "CorruptedMirror";

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = null;

        anim= GetComponent<Animator>(); // disable animator at first
        anim.enabled= false;

        source = GetComponent<AudioSource>();

        swan.SetActive(false);

        if (PlayerPrefs.HasKey(playerPrefKey))
        {
            DisableMirror();
        }
    }

    private void DisableMirror()
    {
        interactable = false; // has been interacted with before
        swan.SetActive(true);
        rend.enabled = false;
    }

    private void Update()
    {
        if(onObject && Input.GetKeyDown(KeyCode.E) && interactable)
        {
            anim.enabled = true;
            StartCoroutine(Wait(delay));

            PlayerPrefs.SetInt(playerPrefKey, 1);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        onObject = true;
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        onObject = false;
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        source.Play();
        DisableMirror();
    }
}
