using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Modo de conclusão da quest
/// </summary>
public enum QuestCompletionMode
{
    Instant,            // Completa imediatamente ao entrar no trigger
    RequiresMinigame,   // Requer completar um minigame
    RequiresCoins       // Requer gastar moedas (para antenas)
}

/// <summary>
/// QuestTrigger - Detecta interação do jogador e gerencia quests.
/// Pode completar instantaneamente ou redirecionar para minigames.
/// </summary>
public class QuestTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public string questName = "quest_exemplo";
    public QuestCompletionMode completionMode = QuestCompletionMode.Instant;

    [Header("Minigame Settings")]
    [Tooltip("Tipo de minigame (se completionMode = RequiresMinigame)")]
    public MinigameType minigameType = MinigameType.None; // None = usa seleção do GameManager

    [Header("Coin Settings")]
    [Tooltip("Custo em moedas (se completionMode = RequiresCoins)")]
    public int coinCost = 300;

    [Header("Antenna Settings")]
    [Tooltip("Marque se este trigger é para ligar uma antena")]
    public bool isAntenna = false;
    [Tooltip("Referência ao AntennaManager (se for antena)")]
    public AntennaManager antennaManager;

    [Header("Visual/Audio Feedback")]
    public GameObject activationEffect;
    public AudioClip activationSound;
    
    [Header("Optional")]
    public GameObject objectToDestroy;
    public GameObject objectToActivate;

    [Header("Events")]
    public UnityEvent OnQuestStarted;
    public UnityEvent OnQuestCompleted;
    public UnityEvent OnInsufficientCoins;

    // Estado interno
    private bool hasBeenTriggered = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // ==================== DETECÇÃO DE TRIGGER ====================

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !hasBeenTriggered)
        {
            ProcessTrigger();
        }
    }

    // Para jogos 2D (descomente se necessário)
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenTriggered)
        {
            ProcessTrigger();
        }
    }
    */

    // ==================== PROCESSAMENTO ====================

    /// <summary>
    /// Processa o trigger baseado no modo de conclusão
    /// </summary>
    private void ProcessTrigger()
    {
        Debug.Log($"[QuestTrigger] Trigger acionado: {questName} (Mode: {completionMode})");
        
        OnQuestStarted?.Invoke();

        switch (completionMode)
        {
            case QuestCompletionMode.Instant:
                CompleteQuestInstantly();
                break;

            case QuestCompletionMode.RequiresMinigame:
                StartMinigameQuest();
                break;

            case QuestCompletionMode.RequiresCoins:
                TryCompleteWithCoins();
                break;
        }
    }

    // ==================== MODO INSTANT ====================

    /// <summary>
    /// Completa a quest imediatamente
    /// </summary>
    private void CompleteQuestInstantly()
    {
        hasBeenTriggered = true;
        
        if (QuestManager.instance != null)
        {
            QuestManager.instance.CompleteQuest(questName);
        }

        PlayFeedback();
        HandlePostCompletion();
        
        OnQuestCompleted?.Invoke();
        Debug.Log($"[QuestTrigger] Quest '{questName}' completada instantaneamente!");
    }

    // ==================== MODO MINIGAME ====================

    /// <summary>
    /// Inicia um minigame para completar a quest
    /// </summary>
    private void StartMinigameQuest()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[QuestTrigger] GameManager não encontrado! Completando quest instantaneamente.");
            CompleteQuestInstantly();
            return;
        }

        // Registrar callback para quando o minigame terminar
        GameManager.Instance.OnMinigameCompleted.AddListener(OnMinigameFinished);

        // Iniciar o minigame
        GameManager.Instance.StartMinigameForQuest(questName, minigameType);
        
        Debug.Log($"[QuestTrigger] Minigame iniciado para quest '{questName}'");
    }

    /// <summary>
    /// Callback chamado quando o minigame termina
    /// </summary>
    private void OnMinigameFinished(MinigameType type, bool success)
    {
        // Remover listener para evitar múltiplas chamadas
        GameManager.Instance.OnMinigameCompleted.RemoveListener(OnMinigameFinished);

        if (success)
        {
            hasBeenTriggered = true;
            PlayFeedback();
            HandlePostCompletion();
            OnQuestCompleted?.Invoke();
            Debug.Log($"[QuestTrigger] Quest '{questName}' completada via minigame!");
        }
        else
        {
            Debug.Log($"[QuestTrigger] Minigame falhou. Quest '{questName}' não completada.");
        }
    }

    // ==================== MODO COINS (ANTENAS) ====================

    /// <summary>
    /// Tenta completar gastando moedas (para antenas)
    /// </summary>
    private void TryCompleteWithCoins()
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[QuestTrigger] CurrencyManager não encontrado!");
            return;
        }

        if (CurrencyManager.Instance.HasEnoughCoins(coinCost))
        {
            // Gastar as moedas
            CurrencyManager.Instance.SpendCoins(coinCost);
            hasBeenTriggered = true;

            // Se for antena, notificar o AntennaManager
            if (isAntenna && antennaManager != null)
            {
                antennaManager.ActivateAntenna(this);
            }
            else if (QuestManager.instance != null)
            {
                QuestManager.instance.CompleteQuest(questName);
            }

            PlayFeedback();
            HandlePostCompletion();
            OnQuestCompleted?.Invoke();

            Debug.Log($"[QuestTrigger] Quest '{questName}' completada! -{coinCost} moedas");
        }
        else
        {
            int currentCoins = CurrencyManager.Instance.GetCoins();
            Debug.Log($"[QuestTrigger] Moedas insuficientes! Precisa: {coinCost}, Tem: {currentCoins}");
            OnInsufficientCoins?.Invoke();
        }
    }

    // ==================== FEEDBACK & CLEANUP ====================

    /// <summary>
    /// Reproduz efeitos visuais e sonoros
    /// </summary>
    private void PlayFeedback()
    {
        if (activationEffect != null)
        {
            Instantiate(activationEffect, transform.position, Quaternion.identity);
        }

        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }

    /// <summary>
    /// Processa objetos após conclusão
    /// </summary>
    private void HandlePostCompletion()
    {
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }

    // ==================== MÉTODOS PÚBLICOS ====================

    /// <summary>
    /// Força a conclusão da quest (chamado externamente)
    /// </summary>
    public void ForceComplete()
    {
        if (!hasBeenTriggered)
        {
            CompleteQuestInstantly();
        }
    }

    /// <summary>
    /// Reseta o trigger para poder ser acionado novamente
    /// </summary>
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
    }

    /// <summary>
    /// Verifica se o jogador pode completar esta quest (tem moedas suficientes)
    /// </summary>
    public bool CanComplete()
    {
        if (completionMode != QuestCompletionMode.RequiresCoins)
            return true;

        return CurrencyManager.Instance != null && 
               CurrencyManager.Instance.HasEnoughCoins(coinCost);
    }
}