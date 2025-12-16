using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI que exibe o progresso das antenas.
/// </summary>
public class AntennaProgressUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Text progressTextLegacy;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Image[] antennaIcons; // Ícones individuais de cada antena

    [Header("Visual Settings")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private string format = "Antenas: {0}/{1}";

    private void Start()
    {
        if (AntennaManager.Instance != null)
        {
            AntennaManager.Instance.OnAntennaActivated.AddListener(OnAntennaActivated);
            AntennaManager.Instance.OnAllAntennasActivated.AddListener(OnAllActivated);

            // Atualizar display inicial
            UpdateDisplay(
                AntennaManager.Instance.GetActivatedCount(),
                AntennaManager.Instance.GetTotalRequired()
            );
        }
        else
        {
            Debug.LogWarning("[AntennaProgressUI] AntennaManager não encontrado!");
        }
    }

    private void OnDestroy()
    {
        if (AntennaManager.Instance != null)
        {
            AntennaManager.Instance.OnAntennaActivated.RemoveListener(OnAntennaActivated);
            AntennaManager.Instance.OnAllAntennasActivated.RemoveListener(OnAllActivated);
        }
    }

    /// <summary>
    /// Chamado quando uma antena é ativada
    /// </summary>
    private void OnAntennaActivated(int current, int total)
    {
        UpdateDisplay(current, total);
    }

    /// <summary>
    /// Chamado quando todas as antenas são ativadas
    /// </summary>
    private void OnAllActivated()
    {
        Debug.Log("[AntennaProgressUI] Todas as antenas ativadas!");
        // Pode adicionar animação especial aqui
    }

    /// <summary>
    /// Atualiza o display de progresso
    /// </summary>
    private void UpdateDisplay(int current, int total)
    {
        // Atualizar texto
        string displayText = string.Format(format, current, total);

        if (progressText != null)
        {
            progressText.text = displayText;
        }

        if (progressTextLegacy != null)
        {
            progressTextLegacy.text = displayText;
        }

        // Atualizar slider
        if (progressSlider != null)
        {
            progressSlider.maxValue = total;
            progressSlider.value = current;
        }

        // Atualizar ícones individuais
        UpdateAntennaIcons(current);
    }

    /// <summary>
    /// Atualiza os ícones visuais das antenas
    /// </summary>
    private void UpdateAntennaIcons(int activatedCount)
    {
        if (antennaIcons == null) return;

        for (int i = 0; i < antennaIcons.Length; i++)
        {
            if (antennaIcons[i] != null)
            {
                antennaIcons[i].color = (i < activatedCount) ? activeColor : inactiveColor;
            }
        }
    }
}
