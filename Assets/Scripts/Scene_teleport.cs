using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_teleport : MonoBehaviour
{
    [SerializeField]
    private string sceneName; // Name of the scene to load.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is tagged "Player"
        if (collision.CompareTag("Player"))
        {
            // Load the scene specified by sceneName.
            SceneManager.LoadScene(sceneName);
        }
    }
}
