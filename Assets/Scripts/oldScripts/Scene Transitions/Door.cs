using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected string nextSceneName;
    [SerializeField] protected Sprite highlightedPic;
    [SerializeField] protected float targetLoc; // target location after entering door
    public static string doorPref = "DoorPref";

    protected Sprite originalPic;
    protected bool isInteracting = false;

    protected GameObject manager;
    protected SceneTransition sceneTransition;

    protected virtual void Start()
    {
        manager = GameObject.Find("Manager");
        sceneTransition = manager.GetComponent<SceneTransition>();

        originalPic = GetComponent<SpriteRenderer>().sprite;

        if (!GetComponent<Collider2D>())
        {
            Debug.Log("YOU FORGOT to add COLLIDER2D SHABI");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //highlight it
        GetComponent<SpriteRenderer>().sprite = highlightedPic;
        isInteracting = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //cancel highlight
        GetComponent<SpriteRenderer>().sprite = originalPic;
        isInteracting = false;
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isInteracting)
        {
            print("Open door!");
            PlayerPrefs.SetFloat(doorPref, targetLoc); // set target location for player
            sceneTransition.SwitchScene(nextSceneName);
        }
    }

}
