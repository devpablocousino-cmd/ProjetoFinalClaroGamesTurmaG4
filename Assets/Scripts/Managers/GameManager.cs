using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Tipos de minigame disponíveis
/// </summary>
public enum MinigameType
{
    None,
    MiniRace,
    Maze
}

/// <summary>
/// Modo de seleção de minigame
/// </summary>
public enum MinigameSelectionMode
{
    Random,         // Aleatório entre os disponíveis
    Sequential,     // Em ordem fixa (MiniRace -> Maze -> MiniRace...)
    PlayerChoice,   // Jogador escolhe (mostra UI)
    Specific        // Definido no QuestTrigger
}

/// <summary>
/// GameManager central que coordena minigames, quests e sistema de jogo.
/// Singleton persistente entre cenas.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    private static GameManager instance;
    public static GameManager Instance => instance;

    // ==================== CONFIGURAÇÕES ====================
    [Header("Minigame Selection")]
    [SerializeField] private MinigameSelectionMode selectionMode = MinigameSelectionMode.Sequential;
    
    [Header("Minigame References")]
    [SerializeField] private GameObject miniRaceArea;
    [SerializeField] private GameObject mazeArea;
    [SerializeField] private GameObject minigameSelectionUI; // UI para escolha do jogador

    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerSpawnPoint; // Ponto de spawn após minigame

    // ==================== ESTADO ====================
    private MinigameType currentMinigame = MinigameType.None;
    private string pendingQuestName = "";
    private int sequentialIndex = 0;
    private bool isMinigameActive = false;

    // ==================== EVENTOS ====================
    [Header("Events")]
    public UnityEvent<MinigameType> OnMinigameStarted = new UnityEvent<MinigameType>();
    public UnityEvent<MinigameType, bool> OnMinigameCompleted = new UnityEvent<MinigameType, bool>(); // tipo, sucesso
    public UnityEvent<string> OnQuestCompletedByMinigame = new UnityEvent<string>();

    // ==================== LIFECYCLE ====================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Encontrar player se não configurado
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // ==================== MINIGAME MANAGEMENT ====================

    /// <summary>
    /// Inicia um minigame associado a uma quest.
    /// </summary>
    /// <param name="questName">Nome da quest que iniciou</param>
    /// <param name="specificType">Tipo específico (se selectionMode = Specific)</param>
    public void StartMinigameForQuest(string questName, MinigameType specificType = MinigameType.None)
    {
        if (isMinigameActive)
        {
            Debug.LogWarning("[GameManager] Já existe um minigame ativo!");
            return;
        }

        pendingQuestName = questName;
        MinigameType selectedType = SelectMinigame(specificType);

        if (selectedType == MinigameType.None)
        {
            Debug.LogError("[GameManager] Nenhum minigame selecionado!");
            return;
        }

        StartMinigame(selectedType);
    }

    /// <summary>
    /// Seleciona qual minigame será jogado baseado no modo configurado
    /// </summary>
    private MinigameType SelectMinigame(MinigameType specificType)
    {
        switch (selectionMode)
        {
            case MinigameSelectionMode.Random:
                return (MinigameType)(Random.Range(1, 3)); // 1 = MiniRace, 2 = Maze

            case MinigameSelectionMode.Sequential:
                sequentialIndex++;
                return (sequentialIndex % 2 == 1) ? MinigameType.MiniRace : MinigameType.Maze;

            case MinigameSelectionMode.PlayerChoice:
                ShowMinigameSelectionUI();
                return MinigameType.None; // UI vai chamar StartMinigame depois

            case MinigameSelectionMode.Specific:
                return specificType != MinigameType.None ? specificType : MinigameType.MiniRace;

            default:
                return MinigameType.MiniRace;
        }
    }

    /// <summary>
    /// Mostra UI para o jogador escolher o minigame
    /// </summary>
    private void ShowMinigameSelectionUI()
    {
        if (minigameSelectionUI != null)
        {
            minigameSelectionUI.SetActive(true);
            // UI deve ter botões que chamam SelectMinigameFromUI(MinigameType)
        }
        else
        {
            // Fallback: escolhe aleatório
            StartMinigame((MinigameType)(Random.Range(1, 3)));
        }
    }

    /// <summary>
    /// Chamado pela UI quando o jogador escolhe um minigame
    /// </summary>
    public void SelectMinigameFromUI(int typeIndex)
    {
        if (minigameSelectionUI != null)
            minigameSelectionUI.SetActive(false);

        StartMinigame((MinigameType)typeIndex);
    }

    /// <summary>
    /// Inicia o minigame selecionado
    /// </summary>
    private void StartMinigame(MinigameType type)
    {
        currentMinigame = type;
        isMinigameActive = true;

        Debug.Log($"[GameManager] Iniciando minigame: {type}");

        switch (type)
        {
            case MinigameType.MiniRace:
                ActivateMiniRace();
                break;
            case MinigameType.Maze:
                ActivateMaze();
                break;
        }

        OnMinigameStarted.Invoke(type);
    }

    /// <summary>
    /// Ativa o minigame MiniRace
    /// </summary>
    private void ActivateMiniRace()
    {
        if (miniRaceArea != null)
        {
            miniRaceArea.SetActive(true);
            Debug.Log("[GameManager] MiniRace ativado");
        }
        else
        {
            Debug.LogError("[GameManager] miniRaceArea não configurado!");
        }
    }

    /// <summary>
    /// Ativa o minigame Maze
    /// </summary>
    private void ActivateMaze()
    {
        if (mazeArea != null)
        {
            mazeArea.SetActive(true);
            Debug.Log("[GameManager] Maze ativado");
        }
        else
        {
            Debug.LogError("[GameManager] mazeArea não configurado!");
        }
    }

    /// <summary>
    /// Chamado quando um minigame é completado.
    /// Este método deve ser chamado pelo script do minigame (MazeExitPoint, etc.)
    /// </summary>
    /// <param name="success">Se o jogador completou com sucesso</param>
    /// <param name="coinsEarned">Moedas ganhas no minigame</param>
    public void OnMinigameComplete(bool success, int coinsEarned = 0)
    {
        if (!isMinigameActive)
        {
            Debug.LogWarning("[GameManager] OnMinigameComplete chamado sem minigame ativo");
            return;
        }

        Debug.Log($"[GameManager] Minigame {currentMinigame} completado. Sucesso: {success}, Moedas: {coinsEarned}");

        // Adicionar moedas ganhas
        if (coinsEarned > 0 && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinsEarned);
        }

        // Disparar evento de minigame completado
        OnMinigameCompleted.Invoke(currentMinigame, success);

        // Se completou com sucesso, completar a quest pendente
        if (success && !string.IsNullOrEmpty(pendingQuestName))
        {
            CompleteQuestFromMinigame(pendingQuestName);
        }

        // Limpar estado
        isMinigameActive = false;
        currentMinigame = MinigameType.None;
        pendingQuestName = "";

        // Desativar áreas de minigame
        DeactivateAllMinigames();
    }

    /// <summary>
    /// Completa uma quest após minigame bem-sucedido
    /// </summary>
    private void CompleteQuestFromMinigame(string questName)
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.CompleteQuest(questName);
            OnQuestCompletedByMinigame.Invoke(questName);
            Debug.Log($"[GameManager] Quest '{questName}' completada via minigame!");
        }
    }

    /// <summary>
    /// Desativa todas as áreas de minigame
    /// </summary>
    private void DeactivateAllMinigames()
    {
        if (miniRaceArea != null) miniRaceArea.SetActive(false);
        if (mazeArea != null) mazeArea.SetActive(false);
    }

    /// <summary>
    /// Cancela o minigame atual (ex: jogador desistiu)
    /// </summary>
    public void CancelMinigame()
    {
        if (!isMinigameActive) return;

        Debug.Log($"[GameManager] Minigame {currentMinigame} cancelado");

        OnMinigameCompleted.Invoke(currentMinigame, false);

        isMinigameActive = false;
        currentMinigame = MinigameType.None;
        pendingQuestName = "";

        DeactivateAllMinigames();
    }

    // ==================== GETTERS ====================

    public bool IsMinigameActive() => isMinigameActive;
    public MinigameType GetCurrentMinigame() => currentMinigame;
    public string GetPendingQuestName() => pendingQuestName;
}
