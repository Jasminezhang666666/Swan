using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void Awake()
    {
        // Ensure that the music object persists between scenes.
        DontDestroyOnLoad(gameObject);

        // Prevent multiple instances of this GameObject.
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
