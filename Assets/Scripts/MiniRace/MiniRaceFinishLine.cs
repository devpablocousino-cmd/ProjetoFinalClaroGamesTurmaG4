using UnityEngine;

/// <summary>
/// Linha de chegada do MiniRace.
/// Detecta quando o jogador cruza e notifica o sistema de pontuação.
/// </summary>
public class MiniRaceFinishLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MiniRaceScoring raceScoring;
    [SerializeField] private MiniRaceEntryPoint entryPoint;

    [Header("Requirements")]
    [SerializeField] private int minimumCheckpoints = 3;        // Mínimo de checkpoints para validar volta

    [Header("Visual")]
    [SerializeField] private GameObject finishEffect;
    [SerializeField] private AudioClip finishSound;

    private AudioSource audioSource;

    private void Start()
    {
        // Auto-encontrar referências
        if (raceScoring == null)
        {
            raceScoring = FindFirstObjectByType<MiniRaceScoring>();
        }

        if (entryPoint == null)
        {
            entryPoint = FindFirstObjectByType<MiniRaceEntryPoint>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && finishSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar se é o jogador/carro
        bool isPlayer = other.CompareTag("Player");

        if (!isPlayer)
        {
            Rigidbody parentRb = other.GetComponentInParent<Rigidbody>();
            if (parentRb != null)
            {
                isPlayer = parentRb.CompareTag("Player");
            }
        }

        if (!isPlayer) return;

        // Verificar se passou pelos checkpoints mínimos
        Pontuacao pontuacao = other.GetComponentInParent<Pontuacao>();
        if (pontuacao != null && pontuacao.GetCheckpoints() < minimumCheckpoints)
        {
            Debug.Log($"[MiniRaceFinishLine] Checkpoints insuficientes: {pontuacao.GetCheckpoints()}/{minimumCheckpoints}");
            return;
        }

        Debug.Log("[MiniRaceFinishLine] Linha de chegada cruzada!");

        // Efeitos
        PlayFinishEffects();

        // Registrar checkpoint final no sistema de pontuação
        if (raceScoring != null)
        {
            raceScoring.RegisterCheckpoint();
            // O MiniRaceScoring vai detectar que completou as voltas e chamar CompleteRace()
        }
    }

    private void PlayFinishEffects()
    {
        if (finishEffect != null)
        {
            Instantiate(finishEffect, transform.position, Quaternion.identity);
        }

        if (finishSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(finishSound);
        }
    }
}
