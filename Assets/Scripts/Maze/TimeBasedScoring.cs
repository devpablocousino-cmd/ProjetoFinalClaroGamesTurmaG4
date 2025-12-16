using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TimeBasedScoring : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerDisplay;
    [SerializeField] private TextMeshProUGUI starsDisplay; // Opcional: mostrar estrelas na UI
    
    [Header("Star Thresholds (Tempo em segundos)")]
    [SerializeField] private float timeFor3Stars = 30f;  // 3 estrelas: até 30s
    [SerializeField] private float timeFor2Stars = 60f;  // 2 estrelas: até 60s
    [SerializeField] private float timeFor1Star = 90f;   // 1 estrela: até 90s
    // Acima de 90s = 0 estrelas
    
    [Header("Points Configuration")]
    [SerializeField] private int pointsPerStar = 100; // Cada estrela = 100 pontos

    [Header("Lifecycle")]
    [SerializeField] private bool autoStartOnStart = false; // Marque se quiser iniciar automaticamente ao ativar o objeto
    
    // Eventos para UI/feedback
    public UnityEvent<int> OnStarsEarned = new UnityEvent<int>();
    public UnityEvent<int> OnBonusCalculated = new UnityEvent<int>();
    
    private float elapsedTime = 0f;
    private bool isActive = false;
    private int lastEarnedStars = 0;
    
    private void Start()
    {
        if (autoStartOnStart)
        {
            StartMazeTimer();
        }
    }
    
    private void Update()
    {
        if (isActive)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
            UpdateStarsPreview(); // Mostra quantas estrelas o jogador ganharia agora
        }
    }
    
    public void StartMazeTimer()
    {
        isActive = true;
        elapsedTime = 0f;
        lastEarnedStars = 0;
        Debug.Log("[TimeBasedScoring] Timer iniciado!");
    }
    
    public void CompleteMazeWithTimeBonus()
    {
        isActive = false;
        
        // Calcular estrelas e bonus
        int stars = CalculateStars();
        int timeBonus = stars * pointsPerStar;
        
        lastEarnedStars = stars;
        
        // Disparar eventos
        OnStarsEarned.Invoke(stars);
        OnBonusCalculated.Invoke(timeBonus);
        
        Debug.Log($"[TimeBasedScoring] Tempo: {elapsedTime:F1}s | Estrelas: {stars} | Bonus: {timeBonus}");
        
        // Verificar se ScoreManager existe
        if (ScoreManager.Instance != null)
        {
            // Adicionar ao score
            ScoreManager.Instance.AddMazeScore(timeBonus);
            
            // Completar o labirinto
            ScoreManager.Instance.CompleteMaze();
        }
        else
        {
            Debug.LogError("[TimeBasedScoring] ScoreManager.Instance é NULL! Certifique-se de que existe um ScoreManager na cena.");
        }
    }
    
    /// <summary>
    /// Calcula quantas estrelas o jogador ganhou baseado no tempo
    /// </summary>
    private int CalculateStars()
    {
        if (elapsedTime <= timeFor3Stars)
            return 3; // Excelente! 300 pontos
        else if (elapsedTime <= timeFor2Stars)
            return 2; // Bom! 200 pontos
        else if (elapsedTime <= timeFor1Star)
            return 1; // Ok! 100 pontos
        else
            return 0; // Muito lento, sem bonus
    }
    
    /// <summary>
    /// Calcula estrelas para um tempo específico (útil para preview)
    /// </summary>
    public int CalculateStarsForTime(float time)
    {
        if (time <= timeFor3Stars) return 3;
        if (time <= timeFor2Stars) return 2;
        if (time <= timeFor1Star) return 1;
        return 0;
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerDisplay != null)
        {
            timerDisplay.text = $"Tempo: {elapsedTime:F1}s";
        }
    }
    
    /// <summary>
    /// Atualiza preview de estrelas durante o jogo
    /// </summary>
    private void UpdateStarsPreview()
    {
        if (starsDisplay != null)
        {
            int currentStars = CalculateStars();
            starsDisplay.text = GetStarsString(currentStars);
            
            // Mudar cor baseado nas estrelas
            starsDisplay.color = currentStars switch
            {
                3 => Color.yellow,  // Ouro
                2 => Color.gray,    // Prata
                1 => new Color(0.8f, 0.5f, 0.2f), // Bronze
                _ => Color.red      // Sem estrelas
            };
        }
    }
    
    /// <summary>
    /// Retorna string visual de estrelas
    /// </summary>
    public string GetStarsString(int stars)
    {
        // Usando asteriscos para compatibilidade com LiberationSans
        // Se quiser símbolos melhores, troque a fonte do StarsText para uma que suporte Unicode
        return stars switch
        {
            3 => "* * *",
            2 => "* * o",
            1 => "* o o",
            _ => "o o o"
        };
    }
    
    // Getters públicos
    public float GetElapsedTime() => elapsedTime;
    public int GetLastEarnedStars() => lastEarnedStars;
    public int GetPointsPerStar() => pointsPerStar;
    public int GetMaxPossibleBonus() => 3 * pointsPerStar; // 300 pontos
}