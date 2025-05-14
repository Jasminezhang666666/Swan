using System.Collections;
using UnityEngine;

public class DressingRoom02_putCase : MonoBehaviour
{
    [Header("Prompt & Input")]
    [SerializeField] private GameObject ePrompt;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Immediate Swap")]
    [SerializeField] private GameObject assignedObject;
    [SerializeField] private GameObject standingSpriteWithoutCase;

    [Header("Delayed Swap")]
    [SerializeField] private float delay = 2f;
    [SerializeField] private GameObject objectToDisableAfterDelay;
    [SerializeField] private GameObject objectToEnableAfterDelay;

    [Header("Player")]
    [Tooltip("Drag the root Player GameObject here")]
    [SerializeField] private GameObject playerObject;

    // internal
    private SpriteRenderer[] _playerSprites;
    private Animator[] _playerAnimators;
    private Player_Ch1 _playerController;
    private bool isPlayerInRange, hasInteracted;

    private void Awake()
    {
        if (playerObject != null)
        {
            _playerSprites = playerObject.GetComponentsInChildren<SpriteRenderer>();
            _playerAnimators = playerObject.GetComponentsInChildren<Animator>();
            _playerController = playerObject.GetComponent<Player_Ch1>();
            if (_playerController == null)
                Debug.LogWarning("Player_Ch1 not found on playerObject!");
        }
        else
        {
            Debug.LogError("playerObject reference is missing!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasInteracted && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ePrompt?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!hasInteracted && other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ePrompt?.SetActive(false);
        }
    }

    private void Update()
    {
        if (!hasInteracted && isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            hasInteracted = true;
            ePrompt?.SetActive(false);

            // Immediate swap
            assignedObject?.SetActive(true);

            // Make every player sprite transparent
            foreach (var sr in _playerSprites)
            {
                var col = sr.color;
                sr.color = new Color(col.r, col.g, col.b, 0f);
            }

            // Stop animations so they don't overwrite color
            foreach (var anim in _playerAnimators)
                anim.enabled = false;

            standingSpriteWithoutCase?.SetActive(true);

            // Disable movement
            _playerController?.DisablePlayerMovement();

            StartCoroutine(DelayedSwap());
        }
    }

    private IEnumerator DelayedSwap()
    {
        yield return new WaitForSeconds(delay);
        objectToDisableAfterDelay?.SetActive(false);
        objectToEnableAfterDelay?.SetActive(true);
        _playerController?.EnablePlayerMovement();
        enabled = false;
    }
}
