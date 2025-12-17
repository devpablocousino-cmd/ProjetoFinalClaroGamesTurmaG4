using UnityEngine;

public enum EstadoCollectClaro
{
    Menu,
    Jogando
}

public class CollectClaroGameManager : MonoBehaviour
{
    public static CollectClaroGameManager Instance;

    [Header("Estado")]
    public EstadoCollectClaro estadoAtual = EstadoCollectClaro.Menu;

    [Header("UI")]
    public GameObject painelInicio;

    [Header("Tempo")]
    public float tempoMax = 30f;
    private float tempoAtual;

    private bool encerrando = false; // evita chamada dupla

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cena começa pausada no menu
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (estadoAtual != EstadoCollectClaro.Jogando)
            return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0f)
        {
            EncerrarMinigame();
        }
    }

    public void IniciarJogo()
    {
        estadoAtual = EstadoCollectClaro.Jogando;

        if (painelInicio)
            painelInicio.SetActive(false);

        tempoAtual = tempoMax;

        // 🔑 libera o jogo
        Time.timeScale = 1f;
    }

    public void EncerrarMinigame()
    {
        if (encerrando)
            return;

        encerrando = true;

        // 🔑 MUITO IMPORTANTE:
        // nunca trocar de cena com timeScale = 0
        Time.timeScale = 1f;

        // volta para o mundo aberto
        MenuManager.Instance.ReturnToWorld();
    }
}
