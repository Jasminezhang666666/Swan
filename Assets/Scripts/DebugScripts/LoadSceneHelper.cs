using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneHelper : MonoBehaviour
{

    public string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    

}
