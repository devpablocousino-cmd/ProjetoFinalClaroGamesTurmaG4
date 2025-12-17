using UnityEngine;

public class OpenWorldMenuManager : MonoBehaviour
{
    public GameObject painelMenu;

    void Awake()
    {
        // SE veio automaticamente do minigame → entra jogando
        if (GameFlowState.retornoAutomaticoDoMinigame)
        {
            GameFlowState.retornoAutomaticoDoMinigame = false;
            painelMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            // Entrada normal no jogo
            painelMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelMenu.activeSelf)
                FecharMenu();
            else
                AbrirMenu();
        }
    }

    public void Jogar()
    {
        FecharMenu();
    }

    void AbrirMenu()
    {
        painelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    void FecharMenu()
    {
        painelMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
