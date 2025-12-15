using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSwitcherInputActions : MonoBehaviour
{
    [Header("Lista de Câmeras (ordem de alternância)")]
    public CinemachineCamera[] cameras;

    [Header("Input Actions")]
    public InputActionReference switchCameraAction;

    private int currentIndex = 0;

    void OnEnable()
    {
        if (switchCameraAction != null)
            switchCameraAction.action.performed += OnSwitchCamera;
    }

    void OnDisable()
    {
        if (switchCameraAction != null)
            switchCameraAction.action.performed -= OnSwitchCamera;
    }

    void Start()
    {
        AtivarCamera(0); // começa sempre na primeira
    }

    void OnSwitchCamera(InputAction.CallbackContext ctx)
    {
        if (cameras == null || cameras.Length == 0)
            return;

        currentIndex++;
        if (currentIndex >= cameras.Length)
            currentIndex = 0;

        AtivarCamera(currentIndex);
    }

    void AtivarCamera(int index)
    {
        if (cameras == null || cameras.Length == 0)
            return;

        for (int i = 0; i < cameras.Length; i++)
        {
            // Quanto maior a prioridade, mais a Cinemachine escolhe essa câmera
            cameras[i].Priority = (i == index) ? 20 : 10;
        }
    }
}
