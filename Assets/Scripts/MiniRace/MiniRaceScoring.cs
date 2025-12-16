using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Sistema de pontuação e tempo para o MiniRace.
/// Similar ao TimeBasedScoring do labirinto.
/// </summary>
public class MiniRaceScoring : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private TextMeshProUGUI lapDisplay;
    [SerializeField] private TextMeshProUGUI positionDisplay;
    [SerializeField] private TextMeshProUGUI coinsDisplay;

    [Header("Race Settings")]
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private int totalCheckpoints = 5;          // Checkpoints por volta

    [Header("Time Bonus Thresholds (segundos)")]
    [SerializeField] private float timeFor3Stars = 60f;         // Ouro
    [SerializeField] private float timeFor2Stars = 90f;         // Prata
    [SerializeField] private float timeFor1Star = 120f;         // Bronze

    [Header("Coin Rewards")]
    [SerializeField] private int coinsFor3Stars = 300;
    [SerializeField] private int coinsFor2Stars = 200;
    [SerializeField] private int coinsFor1Star = 100;
    [SerializeField] private int coinsForCompletion = 50;       // Bônus por completar
    [SerializeField] private int coinsPerCheckpoint = 10;       // Moedas por checkpoint

    [Header("References")]
    [SerializeField] private MiniRaceEntryPoint entryPoint;     // Referência para notificar conclusão

    [Header("Events")]
    public UnityEvent OnRaceStarted;
    public UnityEvent<int> OnLapCompleted;                      // Volta completada (número da volta)
    public UnityEvent<int> OnCheckpointReached;                 // Checkpoint alcançado
    public UnityEvent<int, int> OnRaceCompleted;                // Estrelas, moedas

    // Estado interno
    private float elapsedTime = 0f;
    private bool isRacing = false;
    private int currentLap = 0;
    private int currentCheckpoint = 0;
    private int coinsCollected = 0;
    private int totalCoinsEarned = 0;

    private void Update()
    {
        if (isRacing)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    /// <summary>
    /// Inicia a corrida
    /// </summary>
    public void StartRace()
    {
        isRacing = true;
        elapsedTime = 0f;
        currentLap = 1;
        currentCheckpoint = 0;
        coinsCollected = 0;
        totalCoinsEarned = 0;

        UpdateAllDisplays();
        OnRaceStarted?.Invoke();

        Debug.Log("[MiniRaceScoring] Corrida iniciada!");
    }

    /// <summary>
    /// Para a corrida (sem completar)
    /// </summary>
    public void StopRace()
    {
        isRacing = false;
        Debug.Log($"[MiniRaceScoring] Corrida parada. Tempo: {elapsedTime:F1}s");
    }

    /// <summary>
    /// Registra passagem por um checkpoint
    /// </summary>
    public void RegisterCheckpoint()
    {
        if (!isRacing) return;

        currentCheckpoint++;
        coinsCollected += coinsPerCheckpoint;

        OnCheckpointReached?.Invoke(currentCheckpoint);
        UpdateLapDisplay();

        Debug.Log($"[MiniRaceScoring] Checkpoint {currentCheckpoint}/{totalCheckpoints} - +{coinsPerCheckpoint} moedas");

        // Verificar se completou uma volta
        if (currentCheckpoint >= totalCheckpoints)
        {
            CompleteLap();
        }
    }

    /// <summary>
    /// Completa uma volta
    /// </summary>
    private void CompleteLap()
    {
        currentCheckpoint = 0;
        
        OnLapCompleted?.Invoke(currentLap);
        Debug.Log($"[MiniRaceScoring] Volta {currentLap}/{totalLaps} completada!");

        if (currentLap >= totalLaps)
        {
            // Corrida completa!
            CompleteRace();
        }
        else
        {
            currentLap++;
            UpdateLapDisplay();
        }
    }

    /// <summary>
    /// Completa a corrida e calcula pontuação final
    /// </summary>
    public void CompleteRace()
    {
        isRacing = false;

        // Calcular estrelas baseado no tempo
        int stars = CalculateStars();

        // Calcular moedas totais
        int timeBonus = GetCoinsForStars(stars);
        totalCoinsEarned = coinsCollected + timeBonus + coinsForCompletion;

        Debug.Log($"[MiniRaceScoring] CORRIDA COMPLETA!");
        Debug.Log($"  Tempo: {elapsedTime:F1}s");
        Debug.Log($"  Estrelas: {stars}");
        Debug.Log($"  Moedas coletadas: {coinsCollected}");
        Debug.Log($"  Bônus de tempo: {timeBonus}");
        Debug.Log($"  Bônus de conclusão: {coinsForCompletion}");
        Debug.Log($"  TOTAL: {totalCoinsEarned} moedas");

        // Disparar evento
        OnRaceCompleted?.Invoke(stars, totalCoinsEarned);

        // Notificar o ponto de entrada para finalizar o minigame
        if (entryPoint != null)
        {
            entryPoint.CompleteMiniRace(totalCoinsEarned);
        }
        else
        {
            // Fallback: notificar GameManager diretamente
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMinigameComplete(true, totalCoinsEarned);
            }

            // Adicionar moedas
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(totalCoinsEarned);
            }
        }
    }

    /// <summary>
    /// Adiciona moedas coletadas durante a corrida
    /// </summary>
    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        UpdateCoinsDisplay();
        Debug.Log($"[MiniRaceScoring] +{amount} moedas. Total coletado: {coinsCollected}");
    }

    /// <summary>
    /// Calcula estrelas baseado no tempo total
    /// </summary>
    private int CalculateStars()
    {
        if (elapsedTime <= timeFor3Stars) return 3;
        if (elapsedTime <= timeFor2Stars) return 2;
        if (elapsedTime <= timeFor1Star) return 1;
        return 0;
    }

    /// <summary>
    /// Retorna moedas de bônus por estrelas
    /// </summary>
    private int GetCoinsForStars(int stars)
    {
        return stars switch
        {
            3 => coinsFor3Stars,
            2 => coinsFor2Stars,
            1 => coinsFor1Star,
            _ => 0
        };
    }

    // ==================== UI UPDATES ====================

    private void UpdateAllDisplays()
    {
        UpdateTimerDisplay();
        UpdateLapDisplay();
        UpdateCoinsDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplay != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            timerDisplay.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }

    private void UpdateLapDisplay()
    {
        if (lapDisplay != null)
        {
            lapDisplay.text = $"Volta {currentLap}/{totalLaps}";
        }
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsDisplay != null)
        {
            coinsDisplay.text = $"Moedas: {coinsCollected}";
        }
    }

    // ==================== GETTERS ====================

    public float GetElapsedTime() => elapsedTime;
    public int GetCurrentLap() => currentLap;
    public int GetTotalLaps() => totalLaps;
    public int GetCurrentCheckpoint() => currentCheckpoint;
    public int GetCoinsCollected() => coinsCollected;
    public int GetTotalCoinsEarned() => totalCoinsEarned;
    public bool IsRacing() => isRacing;

    public string GetTimeString()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public string GetStarsString(int stars)
    {
        return stars switch
        {
            3 => "★ ★ ★",
            2 => "★ ★ ☆",
            1 => "★ ☆ ☆",
            _ => "☆ ☆ ☆"
        };
    }
}
