using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Gerencia o sistema de antenas.
/// Controla quantas antenas foram ativadas e o custo de ativação.
/// </summary>
public class AntennaManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    private static AntennaManager instance;
    public static AntennaManager Instance => instance;

    // ==================== CONFIGURAÇÕES ====================
    [Header("Antenna Settings")]
    [SerializeField] private int totalAntennasRequired = 3;
    [SerializeField] private int costPerAntenna = 300;
    [SerializeField] private string antennaQuestName = "quest_antenas";

    [Header("Visual References")]
    [SerializeField] private List<GameObject> antennaVisuals; // Objetos visuais das antenas
    [SerializeField] private Material antennaOnMaterial;       // Material quando ligada
    [SerializeField] private Material antennaOffMaterial;      // Material quando desligada

    // ==================== ESTADO ====================
    private int antennasActivated = 0;
    private List<QuestTrigger> activatedAntennas = new List<QuestTrigger>();

    // ==================== EVENTOS ====================
    [Header("Events")]
    public UnityEvent<int, int> OnAntennaActivated = new UnityEvent<int, int>(); // atual, total
    public UnityEvent OnAllAntennasActivated = new UnityEvent();
    public UnityEvent<int> OnAntennaProgress = new UnityEvent<int>(); // porcentagem 0-100

    // ==================== LIFECYCLE ====================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log($"[AntennaManager] Inicializado. Antenas necessárias: {totalAntennasRequired}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateVisualsAll();
    }

    // ==================== MÉTODOS PÚBLICOS ====================

    /// <summary>
    /// Tenta ativar uma antena (chamado pelo QuestTrigger)
    /// </summary>
    /// <param name="antennaTrigger">O QuestTrigger da antena</param>
    /// <returns>True se ativou com sucesso</returns>
    public bool ActivateAntenna(QuestTrigger antennaTrigger)
    {
        // Verificar se já foi ativada
        if (activatedAntennas.Contains(antennaTrigger))
        {
            Debug.LogWarning("[AntennaManager] Esta antena já foi ativada!");
            return false;
        }

        // Verificar se tem moedas (a verificação principal é feita no QuestTrigger)
        // Aqui apenas registramos a ativação

        antennasActivated++;
        activatedAntennas.Add(antennaTrigger);

        Debug.Log($"[AntennaManager] Antena ativada! Progresso: {antennasActivated}/{totalAntennasRequired}");

        // Atualizar visual da antena
        int antennaIndex = activatedAntennas.Count - 1;
        UpdateAntennaVisual(antennaIndex, true);

        // Disparar eventos
        OnAntennaActivated.Invoke(antennasActivated, totalAntennasRequired);
        OnAntennaProgress.Invoke(GetProgressPercentage());

        // Verificar se completou todas
        if (antennasActivated >= totalAntennasRequired)
        {
            CompleteAntennaQuest();
        }

        return true;
    }

    /// <summary>
    /// Verifica se o jogador pode ativar uma antena (tem moedas suficientes)
    /// </summary>
    public bool CanActivateAntenna()
    {
        if (CurrencyManager.Instance == null) return false;
        return CurrencyManager.Instance.HasEnoughCoins(costPerAntenna);
    }

    /// <summary>
    /// Retorna o custo para ativar uma antena
    /// </summary>
    public int GetAntennaCost()
    {
        return costPerAntenna;
    }

    /// <summary>
    /// Retorna quantas antenas foram ativadas
    /// </summary>
    public int GetActivatedCount()
    {
        return antennasActivated;
    }

    /// <summary>
    /// Retorna o total de antenas necessárias
    /// </summary>
    public int GetTotalRequired()
    {
        return totalAntennasRequired;
    }

    /// <summary>
    /// Retorna o progresso em porcentagem (0-100)
    /// </summary>
    public int GetProgressPercentage()
    {
        if (totalAntennasRequired <= 0) return 100;
        return Mathf.RoundToInt((float)antennasActivated / totalAntennasRequired * 100f);
    }

    /// <summary>
    /// Retorna se todas as antenas foram ativadas
    /// </summary>
    public bool AreAllAntennasActivated()
    {
        return antennasActivated >= totalAntennasRequired;
    }

    /// <summary>
    /// Retorna string de progresso formatada (ex: "2/3")
    /// </summary>
    public string GetProgressString()
    {
        return $"{antennasActivated}/{totalAntennasRequired}";
    }

    // ==================== MÉTODOS PRIVADOS ====================

    /// <summary>
    /// Completa a quest das antenas quando todas são ativadas
    /// </summary>
    private void CompleteAntennaQuest()
    {
        Debug.Log("[AntennaManager] TODAS AS ANTENAS ATIVADAS! Quest completada!");

        if (QuestManager.instance != null)
        {
            QuestManager.instance.CompleteQuest(antennaQuestName);
        }

        OnAllAntennasActivated.Invoke();
    }

    /// <summary>
    /// Atualiza o visual de uma antena específica
    /// </summary>
    private void UpdateAntennaVisual(int index, bool isOn)
    {
        if (antennaVisuals == null || index >= antennaVisuals.Count) return;

        GameObject antenna = antennaVisuals[index];
        if (antenna == null) return;

        Renderer renderer = antenna.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = isOn ? antennaOnMaterial : antennaOffMaterial;
        }

        // Opcional: ativar/desativar efeitos de luz
        Light light = antenna.GetComponentInChildren<Light>();
        if (light != null)
        {
            light.enabled = isOn;
        }
    }

    /// <summary>
    /// Atualiza o visual de todas as antenas
    /// </summary>
    private void UpdateVisualsAll()
    {
        if (antennaVisuals == null) return;

        for (int i = 0; i < antennaVisuals.Count; i++)
        {
            bool isActivated = i < antennasActivated;
            UpdateAntennaVisual(i, isActivated);
        }
    }

    /// <summary>
    /// Reseta o progresso das antenas (use com cuidado)
    /// </summary>
    public void ResetProgress()
    {
        antennasActivated = 0;
        activatedAntennas.Clear();
        UpdateVisualsAll();

        if (QuestManager.instance != null)
        {
            QuestManager.instance.ResetQuest(antennaQuestName);
        }

        Debug.Log("[AntennaManager] Progresso resetado");
    }
}
