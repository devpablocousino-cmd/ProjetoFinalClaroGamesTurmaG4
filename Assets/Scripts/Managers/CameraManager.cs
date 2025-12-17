using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera cityCamera;
    [SerializeField] private Camera mazeCamera;

    [Header("Cinemachine Virtual Cameras")]
    [Tooltip("Câmera virtual Cinemachine da cidade (FreeLook ou outra)")]
    [SerializeField] private CinemachineCamera cityCinemachineCamera;
    
    [Tooltip("Câmera virtual Cinemachine do labirinto")]
    [SerializeField] private CinemachineCamera mazeCinemachineCamera;

    [Header("Priority Settings")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;

    [Header("MiniRace Camera")]
    [Tooltip("Câmera Cinemachine do MiniRace")]
    [SerializeField] private CinemachineCamera miniRaceCinemachineCamera;

    private static CameraManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Garantir que a câmera da cidade está ativa no início
        SetCityCamera();        
    }

    /// <summary>
    /// Ativa a câmera da cidade e desativa todas as outras
    /// </summary>
    public void SetCityCamera()
    {
        Debug.Log("[CameraManager] Ativando Camera da Cidade");

        // Desativar TODAS as câmeras de minigame primeiro
        DeactivateAllMinigameCameras();

        // Ativar câmera da cidade
        if (cityCamera != null)
        {
            cityCamera.gameObject.SetActive(true);
        }

        // Configurar prioridades Cinemachine
        if (cityCinemachineCamera != null)
        {
            cityCinemachineCamera.gameObject.SetActive(true);
            cityCinemachineCamera.Priority = activePriority;
        }
    }

    /// <summary>
    /// Ativa a câmera do labirinto e desativa a da cidade
    /// </summary>
    public void SetMazeCamera()
    {
        Debug.Log("[CameraManager] Ativando Camera do Labirinto");

        // Desativar câmera da cidade
        if (cityCinemachineCamera != null)
        {
            cityCinemachineCamera.Priority = inactivePriority;
            cityCinemachineCamera.gameObject.SetActive(false);
        }

        // Desativar MiniRace se estiver ativa
        if (miniRaceCinemachineCamera != null)
        {
            miniRaceCinemachineCamera.Priority = inactivePriority;
            miniRaceCinemachineCamera.gameObject.SetActive(false);
        }

        // Ativar câmera do labirinto
        if (mazeCamera != null)
        {
            mazeCamera.gameObject.SetActive(true);
        }

        if (mazeCinemachineCamera != null)
        {
            mazeCinemachineCamera.gameObject.SetActive(true);
            mazeCinemachineCamera.Priority = activePriority;
        }
    }

    /// <summary>
    /// Ativa a câmera do MiniRace
    /// </summary>
    public void SetMiniRaceCamera()
    {
        Debug.Log("[CameraManager] Ativando Camera do MiniRace");

        // Desativar câmera da cidade
        if (cityCinemachineCamera != null)
        {
            cityCinemachineCamera.Priority = inactivePriority;
            cityCinemachineCamera.gameObject.SetActive(false);
        }

        // Desativar câmera do labirinto
        if (mazeCinemachineCamera != null)
        {
            mazeCinemachineCamera.Priority = inactivePriority;
            mazeCinemachineCamera.gameObject.SetActive(false);
        }

        if (mazeCamera != null)
        {
            mazeCamera.gameObject.SetActive(false);
        }

        // Ativar MiniRace
        if (miniRaceCinemachineCamera != null)
        {
            miniRaceCinemachineCamera.gameObject.SetActive(true);
            miniRaceCinemachineCamera.Priority = activePriority;
        }
    }

    /// <summary>
    /// Desativa todas as câmeras de minigame (Maze e MiniRace)
    /// </summary>
    public void DeactivateAllMinigameCameras()
    {
        Debug.Log("[CameraManager] Desativando todas as câmeras de minigame");

        // Desativar câmera do labirinto
        if (mazeCamera != null)
        {
            mazeCamera.gameObject.SetActive(false);
        }

        if (mazeCinemachineCamera != null)
        {
            mazeCinemachineCamera.Priority = inactivePriority;
            mazeCinemachineCamera.gameObject.SetActive(false);
        }

        // Desativar câmera do MiniRace
        if (miniRaceCinemachineCamera != null)
        {
            miniRaceCinemachineCamera.Priority = inactivePriority;
            miniRaceCinemachineCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Força a desativação de uma câmera Cinemachine específica por nome
    /// Útil para debug ou casos especiais
    /// </summary>
    public void ForceDeactivateCinemachineCamera(string cameraName)
    {
        CinemachineCamera[] allCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in allCams)
        {
            if (cam.gameObject.name.Contains(cameraName))
            {
                cam.Priority = inactivePriority;
                cam.gameObject.SetActive(false);
                Debug.Log($"[CameraManager] Câmera '{cam.gameObject.name}' forçadamente desativada");
            }
        }
    }

    public static CameraManager Instance => instance;
}