using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Estado de uma quest
/// </summary>
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

/// <summary>
/// Dados de uma quest
/// </summary>
[System.Serializable]
public class QuestData
{
    public string questName;
    public QuestState state = QuestState.NotStarted;
    public int currentProgress = 0;
    public int requiredProgress = 1;

    public bool IsCompleted => state == QuestState.Completed;
    public float ProgressPercentage => requiredProgress > 0 ? (float)currentProgress / requiredProgress * 100f : 0f;
}

/// <summary>
/// QuestManager - Gerencia todas as quests do jogo.
/// Singleton persistente entre cenas.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // Dicionário de quests
    private Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

    // Eventos
    [Header("Events")]
    public UnityEvent<string> OnQuestStarted = new UnityEvent<string>();
    public UnityEvent<string> OnQuestCompleted = new UnityEvent<string>();
    public UnityEvent<string, int, int> OnQuestProgressUpdated = new UnityEvent<string, int, int>(); // nome, atual, total

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[QuestManager] Inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==================== MÉTODOS DE QUEST ====================

    /// <summary>
    /// Inicia uma quest
    /// </summary>
    public void StartQuest(string questName, int requiredProgress = 1)
    {
        if (!quests.ContainsKey(questName))
        {
            quests[questName] = new QuestData
            {
                questName = questName,
                state = QuestState.InProgress,
                currentProgress = 0,
                requiredProgress = requiredProgress
            };
            Debug.Log($"[QuestManager] Quest iniciada: {questName} (0/{requiredProgress})");
            OnQuestStarted.Invoke(questName);
        }
    }

    /// <summary>
    /// Completa uma quest imediatamente
    /// </summary>
    public void CompleteQuest(string questName)
    {
        if (!quests.ContainsKey(questName))
        {
            quests[questName] = new QuestData
            {
                questName = questName,
                state = QuestState.Completed,
                currentProgress = 1,
                requiredProgress = 1
            };
        }
        else
        {
            quests[questName].state = QuestState.Completed;
            quests[questName].currentProgress = quests[questName].requiredProgress;
        }

        Debug.Log($"[QuestManager] Quest completada: {questName}");
        OnQuestCompleted.Invoke(questName);
    }

    /// <summary>
    /// Atualiza o progresso de uma quest
    /// </summary>
    public void UpdateQuestProgress(string questName, int progressAmount = 1)
    {
        if (!quests.ContainsKey(questName))
        {
            StartQuest(questName);
        }

        QuestData quest = quests[questName];
        
        if (quest.state == QuestState.Completed)
        {
            Debug.LogWarning($"[QuestManager] Quest '{questName}' já está completada!");
            return;
        }

        quest.currentProgress += progressAmount;
        
        Debug.Log($"[QuestManager] Progresso atualizado: {questName} ({quest.currentProgress}/{quest.requiredProgress})");
        OnQuestProgressUpdated.Invoke(questName, quest.currentProgress, quest.requiredProgress);

        // Verificar se completou
        if (quest.currentProgress >= quest.requiredProgress)
        {
            CompleteQuest(questName);
        }
    }

    /// <summary>
    /// Verifica se uma quest está completada
    /// </summary>
    public bool IsQuestCompleted(string questName)
    {
        return quests.ContainsKey(questName) && quests[questName].IsCompleted;
    }

    /// <summary>
    /// Verifica se uma quest está em progresso
    /// </summary>
    public bool IsQuestInProgress(string questName)
    {
        return quests.ContainsKey(questName) && quests[questName].state == QuestState.InProgress;
    }

    /// <summary>
    /// Retorna os dados de uma quest
    /// </summary>
    public QuestData GetQuestData(string questName)
    {
        return quests.ContainsKey(questName) ? quests[questName] : null;
    }

    /// <summary>
    /// Retorna o progresso de uma quest como string
    /// </summary>
    public string GetQuestProgressString(string questName)
    {
        if (!quests.ContainsKey(questName)) return "0/0";
        QuestData quest = quests[questName];
        return $"{quest.currentProgress}/{quest.requiredProgress}";
    }

    /// <summary>
    /// Reseta uma quest
    /// </summary>
    public void ResetQuest(string questName)
    {
        if (quests.ContainsKey(questName))
        {
            quests[questName].state = QuestState.NotStarted;
            quests[questName].currentProgress = 0;
            Debug.Log($"[QuestManager] Quest resetada: {questName}");
        }
    }

    /// <summary>
    /// Reseta todas as quests
    /// </summary>
    public void ResetAllQuests()
    {
        quests.Clear();
        Debug.Log("[QuestManager] Todas as quests foram resetadas");
    }

    /// <summary>
    /// Retorna todas as quests completadas
    /// </summary>
    public List<string> GetCompletedQuests()
    {
        List<string> completed = new List<string>();
        foreach (var quest in quests)
        {
            if (quest.Value.IsCompleted)
            {
                completed.Add(quest.Key);
            }
        }
        return completed;
    }
}