using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema centralizado de moedas/currency do jogo.
/// Singleton persistente entre cenas.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    private static CurrencyManager instance;
    public static CurrencyManager Instance => instance;

    // ==================== MOEDAS ====================
    [Header("Currency Settings")]
    [SerializeField] private int startingCoins = 0;
    private int currentCoins = 0;

    // ==================== EVENTOS ====================
    [Header("Events")]
    public UnityEvent<int> OnCoinsChanged = new UnityEvent<int>();
    public UnityEvent<int> OnCoinsAdded = new UnityEvent<int>();
    public UnityEvent<int> OnCoinsSpent = new UnityEvent<int>();
    public UnityEvent OnInsufficientFunds = new UnityEvent();

    // ==================== LIFECYCLE ====================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            currentCoins = startingCoins;
            Debug.Log($"[CurrencyManager] Inicializado com {currentCoins} moedas");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==================== MÉTODOS PÚBLICOS ====================

    /// <summary>
    /// Adiciona moedas ao total do jogador
    /// </summary>
    /// <param name="amount">Quantidade a adicionar (deve ser positivo)</param>
    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[CurrencyManager] Tentativa de adicionar valor inválido: {amount}");
            return;
        }

        currentCoins += amount;
        Debug.Log($"[CurrencyManager] +{amount} moedas. Total: {currentCoins}");

        OnCoinsAdded.Invoke(amount);
        OnCoinsChanged.Invoke(currentCoins);
    }

    /// <summary>
    /// Tenta gastar moedas. Retorna true se bem-sucedido.
    /// </summary>
    /// <param name="amount">Quantidade a gastar</param>
    /// <returns>True se tinha saldo suficiente</returns>
    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[CurrencyManager] Tentativa de gastar valor inválido: {amount}");
            return false;
        }

        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            Debug.Log($"[CurrencyManager] -{amount} moedas gastas. Total: {currentCoins}");

            OnCoinsSpent.Invoke(amount);
            OnCoinsChanged.Invoke(currentCoins);
            return true;
        }
        else
        {
            Debug.Log($"[CurrencyManager] Saldo insuficiente! Precisa: {amount}, Tem: {currentCoins}");
            OnInsufficientFunds.Invoke();
            return false;
        }
    }

    /// <summary>
    /// Verifica se o jogador tem moedas suficientes
    /// </summary>
    public bool HasEnoughCoins(int amount)
    {
        return currentCoins >= amount;
    }

    /// <summary>
    /// Retorna o total atual de moedas
    /// </summary>
    public int GetCoins()
    {
        return currentCoins;
    }

    /// <summary>
    /// Define um valor específico de moedas (use com cuidado)
    /// </summary>
    public void SetCoins(int amount)
    {
        currentCoins = Mathf.Max(0, amount);
        Debug.Log($"[CurrencyManager] Moedas definidas para: {currentCoins}");
        OnCoinsChanged.Invoke(currentCoins);
    }

    /// <summary>
    /// Reseta as moedas para o valor inicial
    /// </summary>
    public void ResetCoins()
    {
        currentCoins = startingCoins;
        Debug.Log($"[CurrencyManager] Moedas resetadas para: {currentCoins}");
        OnCoinsChanged.Invoke(currentCoins);
    }
}
