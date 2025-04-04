using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using AK.Wwise;

public class TransparentDoor : MonoBehaviour
{
    [Header("Fungus Settings")]
    [SerializeField] private Flowchart dialogueFlowchart; // Fungus Flowchart to trigger dialogue.
    [SerializeField] private string fungusBlockName = "";   // Fungus block name. If empty, no dialogue is played.

    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "";         // Next scene name for teleporting. If empty, won't teleport.

    [Header("Audio Settings")]
    public AK.Wwise.Event Door_Open;                        // Sound event for door opening.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Play door open sound.
            if (Door_Open != null)
            {
                Door_Open.Post(this.gameObject);
            }

            // Play the fungus block dialogue if a block name is provided.
            if (dialogueFlowchart != null && !string.IsNullOrEmpty(fungusBlockName))
            {
                dialogueFlowchart.ExecuteBlock(fungusBlockName);
            }

            // Teleport to the next scene only if sceneName is provided.
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("Scene name is empty; not teleporting.");
            }
        }
    }
}