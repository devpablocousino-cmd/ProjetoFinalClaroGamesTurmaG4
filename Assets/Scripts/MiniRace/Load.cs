using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Load : MonoBehaviour
{
    public CinemachineCamera CameraMundoAberto;
    public GameObject MiniRace;
    public GameObject CarroMiniRace;
    public GameObject CanvasMiniRace;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<Rigidbody>().CompareTag("Player"))
        {
            if (gameObject.name == "largada")
            {
                if(other.GetComponentInParent<Pontuacao>().GetCheckpoints() > 2)
                    DescarregarMiniRace();
            }                
            else
                CarregarMiniRace();
        }        
    }

    public void CarregarMiniRace()
    {
        Debug.Log("Loading MiniRace scene...");                        
        CameraMundoAberto.gameObject.SetActive(false);
        CarroMiniRace.SetActive(true);
        CanvasMiniRace.SetActive(true);                    
    }

    public void DescarregarMiniRace()
    {
        Debug.Log("Unloading MiniRace scene...");                        
        CameraMundoAberto.gameObject.SetActive(true);
        CarroMiniRace.SetActive(false);
        CanvasMiniRace.SetActive(false);                    
    }
}
