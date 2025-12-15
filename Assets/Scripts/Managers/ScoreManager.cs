using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    // Singleton Instance
    private static ScoreManager instance;

    // Variáveis de pontuação
    // totalScore: cumulativo (vida inteira da sessão; persiste entre cenas por DontDestroyOnLoad)
    // mazeCurrentScore: pontuação da tentativa atual do labirinto
    private int totalScore = 0;
    private int mazeCurrentScore = 0;
    private int lastMazeScore = 0;

    [Header("Configuração de Pontuação")]
    [SerializeField] private int mazeMaxScore = 300; // Referência para UI (ex: 3 estrelas x 100)

    // Event para notificar quando a pontua��o muda
    public UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
    public UnityEvent<int> OnMazeScoreChanged = new UnityEvent<int>();

    private void Awake()
    {
        // Implementar padr�o Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: persistir entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("[ScoreManager] Sistema de Pontua��o Inicializado");
    }

    /// <summary>
    /// Adiciona pontos ao score atual do labirinto
    /// </summary>
    public void AddMazeScore(int points)
    {
        mazeCurrentScore += points;
        Debug.Log($"[ScoreManager] Pontos Adicionados ao Labirinto: +{points} (Total Maze: {mazeCurrentScore})");

        // Disparar evento
        OnMazeScoreChanged.Invoke(mazeCurrentScore);
    }

    /// <summary>
    /// Subtrai pontos do score atual do labirinto
    /// </summary>
    public void RemoveMazeScore(int points)
    {
        mazeCurrentScore -= points;
        if (mazeCurrentScore < 0) mazeCurrentScore = 0;

        Debug.Log($"[ScoreManager] Pontos Removidos do Labirinto: -{points} (Total: {mazeCurrentScore})");
        OnMazeScoreChanged.Invoke(mazeCurrentScore);
    }

    /// <summary>
    /// Finaliza o minijogo do labirinto e transfere os pontos para o score total
    /// </summary>
    public void CompleteMaze()
    {
        Debug.Log($"[ScoreManager] Labirinto Completado! Score: {mazeCurrentScore}/{mazeMaxScore}");

        // Guardar o score desta tentativa para UI/resultados
        lastMazeScore = mazeCurrentScore;

        // Transferir pontos do labirinto para o score total
        totalScore += mazeCurrentScore;

        // Disparar evento
        OnScoreChanged.Invoke(totalScore);

        Debug.Log($"[ScoreManager] Score Total Agora: {totalScore}");

        // Resetar score do labirinto após concluir para evitar reaproveitar pontos na próxima tentativa
        mazeCurrentScore = 0;
        OnMazeScoreChanged.Invoke(mazeCurrentScore);
    }

    /// <summary>
    /// Reseta o score do labirinto atual
    /// </summary>
    public void ResetMazeScore()
    {
        mazeCurrentScore = 0;
        Debug.Log("[ScoreManager] Score do Labirinto Resetado");
        OnMazeScoreChanged.Invoke(mazeCurrentScore);
    }

    /// <summary>
    /// Reseta tudo (score do labirinto + score total)
    /// </summary>
    public void ResetAllScores()
    {
        mazeCurrentScore = 0;
        totalScore = 0;
        Debug.Log("[ScoreManager] Todos os Scores Resetados");
        OnScoreChanged.Invoke(totalScore);
        OnMazeScoreChanged.Invoke(mazeCurrentScore);
    }

    // ==================== GETTERS ====================

    public int GetTotalScore() => totalScore;

    public int GetMazeCurrentScore() => mazeCurrentScore;

    public int GetMazeMaxScore() => mazeMaxScore;

    public int GetLastMazeScore() => lastMazeScore;

    public float GetMazeScorePercentage()
    {
        if (mazeMaxScore <= 0) return 0f;
        float ratio = (float)mazeCurrentScore / mazeMaxScore;
        return Mathf.Clamp01(ratio) * 100f;
    }

    public static ScoreManager Instance => instance;
}