using UnityEngine;

public class JogoManager : MonoBehaviour
{
    public static JogoManager Instance;

    [Header("Estado do Jogo")]
    public EstadoJogo estadoAtual = EstadoJogo.PreGame;

    [Header("Tempo")]
    public float tempoMaximo = 60f;
    private float tempoAtual;

    [Header("Pontuação")]
    public int pontosParaCompletar = 1000;
    private int pontosAtuais;

    [Header("Minigame (Prefab Root)")]
    public GameObject collectClaroRoot; // root do prefab do minigame

    void Awake()
    {
        // Singleton seguro (SEM DontDestroy aqui)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ResetarJogo();
        Debug.Log("JogoManager iniciado em PreGame");
    }

    void Update()
    {
        if (estadoAtual != EstadoJogo.Jogando)
            return;

        // ===== CONTAGEM DE TEMPO =====
        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0f)
        {
            tempoAtual = 0f;
            FinalizarGameOver();
        }
    }

    // ===============================
    // CONTROLE DO JOGO
    // ===============================

    public void IniciarJogo()
    {
        estadoAtual = EstadoJogo.Jogando;
        tempoAtual = tempoMaximo;
        pontosAtuais = 0;

        Time.timeScale = 1f;

        Debug.Log("MINIGAME INICIADO");
    }

    public void AdicionarPontos(int valor)
    {
        if (estadoAtual != EstadoJogo.Jogando)
            return;

        pontosAtuais += valor;
        Debug.Log($"Pontos atuais: {pontosAtuais}");

        if (pontosAtuais >= pontosParaCompletar)
        {
            FinalizarCompleto();
        }
    }

    void FinalizarGameOver()
    {
        estadoAtual = EstadoJogo.GameOver;
        Time.timeScale = 0f;

        Debug.Log("GAME OVER - TEMPO ESGOTADO");

        EncerrarMinigame();
    }

    void FinalizarCompleto()
    {
        estadoAtual = EstadoJogo.Completo;
        Time.timeScale = 0f;

        Debug.Log("MINIGAME COMPLETO - META ATINGIDA");

        EncerrarMinigame();
    }

    void EncerrarMinigame()
    {
        if (collectClaroRoot != null)
            collectClaroRoot.SetActive(false);

        estadoAtual = EstadoJogo.PreGame;
    }

    void ResetarJogo()
    {
        estadoAtual = EstadoJogo.PreGame;
        tempoAtual = tempoMaximo;
        pontosAtuais = 0;

        Time.timeScale = 0f;

        if (collectClaroRoot != null)
            collectClaroRoot.SetActive(false);
    }

    // ===============================
    // GETTERS (HUD / DEBUG)
    // ===============================

    public float GetTempoAtual() => tempoAtual;
    public int GetPontosAtuais() => pontosAtuais;
}
