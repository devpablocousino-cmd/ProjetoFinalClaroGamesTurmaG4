using UnityEngine;

/// <summary>
/// Checkpoint do MiniRace.
/// Registra passagem do jogador e notifica o sistema de pontuação.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isFinishLine = false;         // Marca se é a linha de chegada
    [SerializeField] private int checkpointIndex = 0;           // Ordem do checkpoint (opcional)
    
    [Header("References")]
    [SerializeField] private MiniRaceScoring raceScoring;       // Sistema de pontuação

    [Header("Visual")]
    [SerializeField] private bool destroyOnPass = true;         // Destruir ao passar
    [SerializeField] private GameObject passEffect;             // Efeito visual ao passar

    private bool hasBeenPassed = false;

    private void Start()
    {
        // Auto-encontrar MiniRaceScoring se não configurado
        if (raceScoring == null)
        {
            raceScoring = FindFirstObjectByType<MiniRaceScoring>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar se é o jogador/carro
        bool isPlayer = other.CompareTag("Player");
        
        // Também verificar no pai (caso o collider seja filho)
        if (!isPlayer)
        {
            Rigidbody parentRb = other.GetComponentInParent<Rigidbody>();
            if (parentRb != null)
            {
                isPlayer = parentRb.CompareTag("Player");
            }
        }

        if (!isPlayer || hasBeenPassed) return;

        hasBeenPassed = true;

        // Registrar no sistema antigo (Pontuacao)
        Pontuacao pontuacao = other.GetComponentInParent<Pontuacao>();
        if (pontuacao != null)
        {
            pontuacao.AdicionarCheckpoint();
            pontuacao.SetUltimoCheckpoint(transform);
        }

        // Registrar no novo sistema de pontuação
        if (raceScoring != null)
        {
            raceScoring.RegisterCheckpoint();
        }

        Debug.Log($"[CheckPoint] Checkpoint {checkpointIndex} passado!");

        // Efeito visual
        if (passEffect != null)
        {
            Instantiate(passEffect, transform.position, Quaternion.identity);
        }

        // Destruir ou desativar
        if (destroyOnPass)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Reseta o checkpoint para poder ser passado novamente
    /// </summary>
    public void ResetCheckpoint()
    {
        hasBeenPassed = false;
        gameObject.SetActive(true);
    }
}
