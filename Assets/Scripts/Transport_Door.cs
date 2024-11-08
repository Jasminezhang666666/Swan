using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AK.Wwise;

public class Transport_Door : MonoBehaviour
{

    [SerializeField] private string sceneName; // Scene name for teleporting
    public AK.Wwise.Event Door_Open;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Door_Open.Post(this.gameObject);

            SceneManager.LoadScene(sceneName);
        }
    }
}
