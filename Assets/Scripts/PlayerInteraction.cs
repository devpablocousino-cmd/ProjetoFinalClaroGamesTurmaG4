using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance;

    private bool interactPressed;

    void Awake()
    {
        Instance = this;
    }

    // ======== TECLADO (Input System ou manual) ========
    public void PressInteract()
    {
        interactPressed = true;
    }

    // ======== BOTÃO UI ========
    public void PressInteractFromUI()
    {
        interactPressed = true;
    }

    // ======== CONSUMO ========
    public bool ConsumeInteract()
    {
        if (!interactPressed)
            return false;

        interactPressed = false;
        return true;
    }
}
