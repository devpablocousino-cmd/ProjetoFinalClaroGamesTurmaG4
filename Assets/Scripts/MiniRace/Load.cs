using Unity.Cinemachine;
using UnityEngine;

public class Load : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineCamera CameraMundoAberto;
    
    [Header("MiniRace Objects")]
    public GameObject MiniRace;
    public GameObject CarroMiniRace;
    public GameObject CanvasMiniRace;

    [Header("Use CameraManager")]
    [Tooltip("Se marcado, usa o CameraManager centralizado para trocar câmeras")]
    public bool useCameraManager = true;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (other.GetComponentInParent<Rigidbody>() != null && other.GetComponentInParent<Rigidbody>().CompareTag("Player")))
        {
            if (gameObject.name == "largada")
            {
                Pontuacao pontuacao = other.GetComponentInParent<Pontuacao>();
                if (pontuacao != null && pontuacao.GetCheckpoints() > 2)
                    DescarregarMiniRace();
            }                
            else
                CarregarMiniRace();
        }        
    }

    public void CarregarMiniRace()
    {
        Debug.Log("[Load] Carregando MiniRace...");
        
        // Usar CameraManager se disponível
        if (useCameraManager && CameraManager.Instance != null)
        {
            CameraManager.Instance.SetMiniRaceCamera();
        }
        else
        {
            // Fallback: desativar câmera do mundo aberto diretamente
            if (CameraMundoAberto != null)
            {
                CameraMundoAberto.gameObject.SetActive(false);
            }
        }
        
        if (CarroMiniRace != null) CarroMiniRace.SetActive(true);
        if (CanvasMiniRace != null) CanvasMiniRace.SetActive(true);
    }

    public void DescarregarMiniRace()
    {
        Debug.Log("[Load] Descarregando MiniRace...");
        
        // Usar CameraManager se disponível
        if (useCameraManager && CameraManager.Instance != null)
        {
            CameraManager.Instance.SetCityCamera();
        }
        else
        {
            // Fallback: ativar câmera do mundo aberto diretamente
            if (CameraMundoAberto != null)
            {
                CameraMundoAberto.gameObject.SetActive(true);
            }
        }
        
        if (CarroMiniRace != null) CarroMiniRace.SetActive(false);
        if (CanvasMiniRace != null) CanvasMiniRace.SetActive(false);
    }
}
