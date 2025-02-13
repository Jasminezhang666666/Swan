using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class credits : MonoBehaviour
{
    private void Update()
    {
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Open");
        }
    }
}
