using UnityEngine;

public class MazeExitPoint : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform cityExitPoint;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject mazeAreaGameObject;

    [Header("Scoring")]
    [SerializeField] private TimeBasedScoring timeScoring;

    [Header("Portal")]
    [SerializeField] private TeleportPoint teleportPoint;

    [Header("Visual Feedback")]
    [SerializeField] private Color highlightColor = Color.green;
    private Color originalColor;
    private Renderer meshRenderer;

    private bool hasCompleted = false;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }

        if (timeScoring == null)
        {
            timeScoring = FindFirstObjectByType<TimeBasedScoring>();
            if (timeScoring != null)
            {
                Debug.Log($"[MazeExitPoint] TimeBasedScoring encontrado automaticamente: {timeScoring.gameObject.name}");
            }
        }

        if (teleportPoint == null)
        {
            teleportPoint = FindFirstObjectByType<TeleportPoint>();
            if (teleportPoint != null)
            {
                Debug.Log($"[MazeExitPoint] TeleportPoint encontrado automaticamente: {teleportPoint.gameObject.name}");
            }
        }

        // Tenta encontrar o player automaticamente se não estiver configurado
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                characterController = player.GetComponent<CharacterController>();
                Debug.Log($"[MazeExitPoint] Player encontrado automaticamente: {player.name}");
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log($"[MazeExitPoint] Algo entrou no trigger: {collision.gameObject.name}, Tag: {collision.tag}");

        if (collision.CompareTag("Player"))
        {
            if (hasCompleted)
            {
                return;
            }

            hasCompleted = true;
            Debug.Log("[MazeExitPoint] Jogador atingiu a saída do labirinto!");

            if (meshRenderer != null)
            {
                meshRenderer.material.color = highlightColor;
            }

            // Calcular moedas ganhas baseado no tempo
            int coinsEarned = 0;
            
            // Completar o minijogo
            if (timeScoring != null)
            {
                timeScoring.CompleteMazeWithTimeBonus();
                coinsEarned = timeScoring.GetLastEarnedStars() * timeScoring.GetPointsPerStar();
            }
            else if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.CompleteMaze();
                coinsEarned = ScoreManager.Instance.GetLastMazeScore();
            }
            else
            {
                Debug.LogError("[MazeExitPoint] ScoreManager.Instance é NULL e não há TimeBasedScoring configurado.");
            }

            // Notificar o GameManager que o minigame foi completado
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMinigameComplete(true, coinsEarned);
            }

            // Sair do labirinto
            ExitMaze();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (meshRenderer != null)
            {
                meshRenderer.material.color = originalColor;
            }
        }
    }

    private void ExitMaze()
    {
        Debug.Log("[MazeExitPoint] Iniciando teletransporte para a cidade...");

        // Se existir TeleportPoint, preferir ele para garantir que o portal volte a ficar disponível
        if (teleportPoint != null)
        {
            teleportPoint.ExitMaze(cityExitPoint);
            return;
        }

        // Validação das referências
        if (cityExitPoint == null)
        {
            Debug.LogError("[MazeExitPoint] ERRO: cityExitPoint não está configurado!");
            return;
        }

        if (playerTransform == null)
        {
            // Tenta encontrar novamente
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                characterController = player.GetComponent<CharacterController>();
            }
            else
            {
                Debug.LogError("[MazeExitPoint] ERRO: playerTransform não encontrado!");
                return;
            }
        }

        // Desabilitar o CharacterController antes de teletransportar
        if (characterController != null)
        {
            characterController.enabled = false;
            Debug.Log("[MazeExitPoint] CharacterController desabilitado");
        }

        // Teletransportar o jogador
        Vector3 targetPosition = cityExitPoint.position;
        Quaternion targetRotation = cityExitPoint.rotation;

        playerTransform.position = targetPosition;
        playerTransform.rotation = targetRotation;

        Debug.Log($"[MazeExitPoint] Jogador movido para: {targetPosition}");

        // Sincroniza as transformações com a física
        Physics.SyncTransforms();

        // Reabilitar o CharacterController após teletransportar
        if (characterController != null)
        {
            characterController.enabled = true;
            Debug.Log("[MazeExitPoint] CharacterController reabilitado");
        }

        // Se a saída foi feita por este script (sem TeleportPoint), ainda assim tenta liberar o portal
        TeleportPoint anyPortal = FindFirstObjectByType<TeleportPoint>();
        if (anyPortal != null)
        {
            anyPortal.ResetPortalState();
        }

        // Desativar a área do labirinto
        if (mazeAreaGameObject != null)
        {
            mazeAreaGameObject.SetActive(false);
            Debug.Log("[MazeExitPoint] Área do labirinto desativada");
        }

        // Restaurar câmera da cidade
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SetCityCamera();
            Debug.Log("[MazeExitPoint] Câmera da cidade ativada");
        }

        Debug.Log("[MazeExitPoint] Teletransporte concluído! Jogador voltou à cidade.");
    }
}