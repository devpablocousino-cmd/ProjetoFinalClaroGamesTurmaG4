using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    private bool playerInside;
    public UnityEvent onInteract;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (!playerInside) return;        
        if (PlayerInteraction.Instance != null &&
            PlayerInteraction.Instance.ConsumeInteract())
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interação realizada!");
        onInteract.Invoke();
    }
}
