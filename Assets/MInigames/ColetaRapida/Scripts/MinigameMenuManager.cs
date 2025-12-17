using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameMenuManager : MonoBehaviour
{
    public GameObject painelMenuMinigame;
    public float tempoDeJogo = 60f;
    public string nomeCenaMundoAberto = "MundoAberto";

    bool jogando = false;
    float timer;

    void Start()
    {
        // AO ENTRAR NO MINIGAME → MENU ABERTO
        painelMenuMinigame.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        // ESC = pause
        if (jogando && Input.GetKeyDown(KeyCode.Escape))
        {
            Pausar();
        }

        if (!jogando) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            FinalizarAutomaticamente();
        }
    }

    // ===== BOTÕES =====

    public void Jogar()
    {
        painelMenuMinigame.SetActive(false);
        Time.timeScale = 1f;
        jogando = true;
        timer = tempoDeJogo;
    }

    public void Sair()
    {
        VoltarParaMundoAberto(false);
    }

    public void Continuar()
    {
        painelMenuMinigame.SetActive(false);
        Time.timeScale = 1f;
    }

    // ===== LÓGICA =====

    void Pausar()
    {
        painelMenuMinigame.SetActive(true);
        Time.timeScale = 0f;
    }

    void FinalizarAutomaticamente()
    {
        VoltarParaMundoAberto(true);
    }

    void VoltarParaMundoAberto(bool automatico)
    {
        Time.timeScale = 1f;

        if (automatico)
            GameFlowState.retornoAutomaticoDoMinigame = true;

        SceneManager.LoadScene(nomeCenaMundoAberto);
    }
}
