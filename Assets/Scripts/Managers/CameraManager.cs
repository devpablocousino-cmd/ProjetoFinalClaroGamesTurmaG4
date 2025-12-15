using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera cityCamera;
    [SerializeField] private Camera mazeCamera;

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
        // Garantir que a c�mera da cidade est� ativa no in�cio
        SetCityCamera();
    }

    /// <summary>
    /// Ativa a c�mera da cidade e desativa a do labirinto
    /// </summary>
    public void SetCityCamera()
    {
        Debug.Log("[CameraManager] Ativando Camera da Cidade");

        // Ativa/desativa os GameObjects inteiros para garantir que os scripts funcionem corretamente
        if (cityCamera != null)
        {
            cityCamera.gameObject.SetActive(true);
        }
        if (mazeCamera != null)
        {
            mazeCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Ativa a c�mera do labirinto e desativa a da cidade
    /// </summary>
    public void SetMazeCamera()
    {
        Debug.Log("[CameraManager] Ativando Camera do Labirinto");

        // Ativa/desativa os GameObjects inteiros para garantir que os scripts funcionem corretamente
        if (cityCamera != null)
        {
            cityCamera.gameObject.SetActive(false);
        }
        if (mazeCamera != null)
        {
            mazeCamera.gameObject.SetActive(true);
        }
    }

    public static CameraManager Instance => instance;
}