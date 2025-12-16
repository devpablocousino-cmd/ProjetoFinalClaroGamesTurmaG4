using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI que exibe o total de moedas e se atualiza automaticamente.
/// </summary>
public class CurrencyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Text coinsTextLegacy; // Para UI.Text legado

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string addCoinsAnimTrigger = "AddCoins";
    [SerializeField] private string spendCoinsAnimTrigger = "SpendCoins";

    [Header("Format")]
    [SerializeField] private string format = "Moedas: {0}";

    private void Start()
    {
        // Registrar nos eventos do CurrencyManager
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged.AddListener(UpdateDisplay);
            CurrencyManager.Instance.OnCoinsAdded.AddListener(OnCoinsAdded);
            CurrencyManager.Instance.OnCoinsSpent.AddListener(OnCoinsSpent);
            
            // Atualizar display inicial
            UpdateDisplay(CurrencyManager.Instance.GetCoins());
        }
        else
        {
            Debug.LogWarning("[CurrencyUI] CurrencyManager não encontrado!");
        }
    }

    private void OnDestroy()
    {
        // Desregistrar eventos
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged.RemoveListener(UpdateDisplay);
            CurrencyManager.Instance.OnCoinsAdded.RemoveListener(OnCoinsAdded);
            CurrencyManager.Instance.OnCoinsSpent.RemoveListener(OnCoinsSpent);
        }
    }

    /// <summary>
    /// Atualiza o texto de moedas
    /// </summary>
    private void UpdateDisplay(int coins)
    {
        string displayText = string.Format(format, coins);

        if (coinsText != null)
        {
            coinsText.text = displayText;
        }

        if (coinsTextLegacy != null)
        {
            coinsTextLegacy.text = displayText;
        }
    }

    /// <summary>
    /// Chamado quando moedas são adicionadas
    /// </summary>
    private void OnCoinsAdded(int amount)
    {
        if (animator != null && !string.IsNullOrEmpty(addCoinsAnimTrigger))
        {
            animator.SetTrigger(addCoinsAnimTrigger);
        }
    }

    /// <summary>
    /// Chamado quando moedas são gastas
    /// </summary>
    private void OnCoinsSpent(int amount)
    {
        if (animator != null && !string.IsNullOrEmpty(spendCoinsAnimTrigger))
        {
            animator.SetTrigger(spendCoinsAnimTrigger);
        }
    }
}
