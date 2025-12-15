using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI mazeScoreText;
    [SerializeField] private TextMeshProUGUI mazePercentageText;

    private bool isSubscribed;

    private void Start()
    {
        // Validar se os elementos de UI foram atribu�dos
        if (totalScoreText == null)
            Debug.LogWarning("[ScoreDisplay] totalScoreText n�o foi atribu�do!");

        if (mazeScoreText == null)
            Debug.LogWarning("[ScoreDisplay] mazeScoreText n�o foi atribu�do!");

        TrySubscribeOrWait();
    }

    private void TrySubscribeOrWait()
    {
        if (ScoreManager.Instance != null)
        {
            Subscribe();
            return;
        }

        StartCoroutine(WaitForScoreManagerThenSubscribe());
    }

    private System.Collections.IEnumerator WaitForScoreManagerThenSubscribe()
    {
        while (ScoreManager.Instance == null)
        {
            yield return null;
        }

        Subscribe();
    }

    private void Subscribe()
    {
        if (isSubscribed || ScoreManager.Instance == null)
            return;

        ScoreManager.Instance.OnScoreChanged.AddListener(UpdateTotalScore);
        ScoreManager.Instance.OnMazeScoreChanged.AddListener(UpdateMazeScore);

        // Atualizar displays iniciais
        UpdateTotalScore(ScoreManager.Instance.GetTotalScore());
        UpdateMazeScore(ScoreManager.Instance.GetMazeCurrentScore());

        isSubscribed = true;
    }

    /// <summary>
    /// Callback chamado quando o score total muda
    /// </summary>
    private void UpdateTotalScore(int newScore)
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Score Total: {newScore}";
            Debug.Log($"[ScoreDisplay] Score Total Atualizado: {newScore}");
        }
    }

    /// <summary>
    /// Callback chamado quando o score do labirinto muda
    /// </summary>
    private void UpdateMazeScore(int newScore)
    {
        if (mazeScoreText != null)
        {
            mazeScoreText.text = $"Maze Score: {newScore}/{ScoreManager.Instance.GetMazeMaxScore()}";
        }

        if (mazePercentageText != null)
        {
            float percentage = ScoreManager.Instance.GetMazeScorePercentage();
            mazePercentageText.text = $"{percentage:F1}%";
        }

        Debug.Log($"[ScoreDisplay] Maze Score Atualizado: {newScore}");
    }

    private void OnDestroy()
    {
        // Desinscrever dos eventos ao destruir
        if (isSubscribed && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.RemoveListener(UpdateTotalScore);
            ScoreManager.Instance.OnMazeScoreChanged.RemoveListener(UpdateMazeScore);
        }

        isSubscribed = false;
    }
}