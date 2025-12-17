using UnityEngine;
using UnityEngine.SceneManagement;

// O NOME AQUI EMBAIXO TEM QUE SER IGUAL AO NOME DO ARQUIVO
public class ClaroManager : MonoBehaviour
{
    // Mudei para ClaroManager para não brigar com o código do seu grupo
    public static ClaroManager instance;

    public Vector3 ultimaPosicao;
    public bool deveReposicionar = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IrParaMinigame(string nomeCena, Vector3 posicaoPlayer)
    {
        ultimaPosicao = posicaoPlayer;
        deveReposicionar = true;
        SceneManager.LoadScene(nomeCena);
    }
}