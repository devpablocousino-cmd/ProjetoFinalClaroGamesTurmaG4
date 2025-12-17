using UnityEngine;
using TMPro;

public class EntregadorManager : MonoBehaviour
{
    [Header("Objetos da Cena")]
    public GameObject pacote;          // Logo da Claro (coletável)
    public GameObject cliente;         // Smartphone (entrega)
    public Transform player;           // Player

    [Header("Área de Spawn")]
    public float tamanhoArena = 6f;
    public float alturaPacote = 0.5f;

    [Header("Distâncias Mínimas")]
    public float distanciaMinCliente = 4f;
    public float distanciaMinPlayer = 3f;
    public float distanciaMinUltimoPacote = 3f;

    [Header("Pontuação")]
    public int pontosPorEntrega = 100;

    [Header("Áudio")]
    public AudioSource audioFX;
    public AudioClip somColeta;
    public AudioClip somEntrega;

    [Header("UI")]
    public TextMeshProUGUI uiPontos;

    // Estado interno
    private bool temPacote = false;
    private int pontuacao = 0;
    private Vector3 ultimaPosicaoPacote;

    void Start()
    {
        temPacote = false;
        pontuacao = 0;

        if (uiPontos != null)
            uiPontos.text = "0";

        cliente.SetActive(false);
        pacote.SetActive(true);

        SpawnarPacote();
    }

    // =========================
    // COLETA DO PACOTE
    // =========================
    public void PegouPacote()
    {
        if (temPacote) return;

        temPacote = true;

        if (audioFX && somColeta)
            audioFX.PlayOneShot(somColeta);

        pacote.SetActive(false);
        cliente.SetActive(true);

        PosicionarCliente();
    }

    // =========================
    // ENTREGA DO PACOTE
    // =========================
    public void EntregouPacote()
    {
        if (!temPacote) return;

        temPacote = false;
        pontuacao += pontosPorEntrega;

        if (uiPontos != null)
            uiPontos.text = pontuacao.ToString();

        if (audioFX && somEntrega)
            audioFX.PlayOneShot(somEntrega);

        cliente.SetActive(false);
        pacote.SetActive(true);

        SpawnarPacote();
    }

    // =========================
    // SPAWN DO PACOTE
    // =========================
    void SpawnarPacote()
    {
        Vector3 posicao;
        int tentativas = 0;

        do
        {
            float x = Random.Range(-tamanhoArena, tamanhoArena);
            float z = Random.Range(-tamanhoArena, tamanhoArena);

            posicao = new Vector3(x, alturaPacote, z);
            tentativas++;

            if (tentativas > 50) break;

        } while (
            Vector3.Distance(posicao, player.position) < distanciaMinPlayer ||
            Vector3.Distance(posicao, cliente.transform.position) < distanciaMinCliente ||
            Vector3.Distance(posicao, ultimaPosicaoPacote) < distanciaMinUltimoPacote
        );

        pacote.transform.position = posicao;
        ultimaPosicaoPacote = posicao;
    }

    // =========================
    // POSICIONA CLIENTE
    // =========================
    void PosicionarCliente()
    {
        Vector3 posicao;
        int tentativas = 0;

        do
        {
            float x = Random.Range(-tamanhoArena, tamanhoArena);
            float z = Random.Range(-tamanhoArena, tamanhoArena);

            posicao = new Vector3(x, alturaPacote, z);
            tentativas++;

            if (tentativas > 50) break;

        } while (
            Vector3.Distance(posicao, player.position) < distanciaMinPlayer
        );

        cliente.transform.position = posicao;
    }
}
