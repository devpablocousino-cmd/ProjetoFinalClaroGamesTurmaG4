using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalizarMinigame : MonoBehaviour
{
    public string nomeCenaMundoAberto = "MundoAberto";

    public void Finalizar()
    {
        // marca que deve voltar jogando
        OpenWorldState.voltandoDoMinigame = true;

        Time.timeScale = 1f; // segurança
        SceneManager.LoadScene(nomeCenaMundoAberto);
    }
}
