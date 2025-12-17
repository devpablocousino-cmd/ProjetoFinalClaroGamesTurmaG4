using UnityEngine;

public class MinigameESC : MonoBehaviour
{
    // Arraste aqui o script que o botão Sair já usa
    public MonoBehaviour scriptDoBotaoSair;

    // Nome EXATO da função chamada pelo botão Sair
    public string nomeMetodoSair = "Finalizar";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (scriptDoBotaoSair != null)
            {
                scriptDoBotaoSair.Invoke(nomeMetodoSair, 0f);
            }
            else
            {
                Debug.LogError("MinigameESC: scriptDoBotaoSair não atribuído");
            }
        }
    }
}
