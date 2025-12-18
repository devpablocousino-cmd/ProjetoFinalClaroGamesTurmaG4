using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarMenu : MonoBehaviour
{
    public void IrParaMenu()
    {
        SceneManager.LoadScene(0); // Carrega a cena de índice 0 (geralmente é o Menu)
    }
}
