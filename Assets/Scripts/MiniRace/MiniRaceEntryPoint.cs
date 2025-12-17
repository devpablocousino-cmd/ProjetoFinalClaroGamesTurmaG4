using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Ponto de entrada para o MiniRace (corrida de kart).
/// Similar ao TeleportPoint do labirinto, mas para o minigame de corrida.
/// O jogador deve se aproximar e pressionar E para entrar.
/// </summary>
public class MiniRaceEntryPoint : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private GameObject playerGameObject;

    [Header("MiniRace References")]
    [SerializeField] private GameObject miniRaceArea;           // Área completa do MiniRace
    [SerializeField] private GameObject carGameObject;          // O carro que o jogador vai controlar
    [SerializeField] private Transform raceStartPoint;          // Ponto de início da corrida
    [SerializeField] private GameObject miniRaceCanvas;         // Canvas/UI do MiniRace

    [Header("Exit References")]
    [SerializeField] private Transform cityReturnPoint;         // Onde o jogador volta após terminar
    [SerializeField] private GameObject cityCanvas;              // Canvas/UI do GamePrincipal

    [Header("Scoring")]
    [SerializeField] private MiniRaceScoring raceScoring;       // Sistema de pontuação da corrida

    [Header("Visual Feedback")]
    [SerializeField] private Color highlightColor = Color.cyan;
    [SerializeField] private GameObject interactionPrompt;       // UI "Pressione E para entrar"
    
    [Header("Settings")]
    [SerializeField] private bool hidePlayerDuringRace = true;  // Esconde o jogador durante a corrida
    [SerializeField] private bool requireConfirmation = true;   // Requer pressionar E para entrar

    [Header("Events")]
    public UnityEvent OnRaceStarted;
    public UnityEvent OnRaceCompleted;
    public UnityEvent<int> OnRaceCompletedWithScore;

    // Estado interno
    private bool isPlayerNear = false;
    private bool isInRace = false;
    private Color originalColor;
    private Renderer meshRenderer;
    private Vector3 playerOriginalPosition;
    private Quaternion playerOriginalRotation;

    private void Start()
    {
        // Garantir que a área do MiniRace está desativada no início
        if (miniRaceArea != null)
        {
            miniRaceArea.SetActive(false);
        }

        if (carGameObject != null)
        {
            carGameObject.SetActive(false);
        }

        if (miniRaceCanvas != null)
        {
            miniRaceCanvas.SetActive(false);
        }

        // Configurar visual
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Auto-encontrar player se não configurado
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCharacterController = player.GetComponent<CharacterController>();
                playerGameObject = player;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !isInRace)
        {
            isPlayerNear = true;

            // Feedback visual
            if (meshRenderer != null)
            {
                meshRenderer.material.color = highlightColor;
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }

            Debug.Log("[MiniRaceEntryPoint] Jogador próximo! Pressione E para iniciar a corrida");

            // Se não requer confirmação, entra automaticamente
            if (!requireConfirmation)
            {
                EnterMiniRace();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (meshRenderer != null)
            {
                meshRenderer.material.color = originalColor;
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Só permite entrar se estiver perto E não estiver já na corrida
        if (isPlayerNear && !isInRace && requireConfirmation)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                EnterMiniRace();
            }
        }
    }

    /// <summary>
    /// Inicia o MiniRace
    /// </summary>
    public void EnterMiniRace()
    {
        if (isInRace)
        {
            Debug.LogWarning("[MiniRaceEntryPoint] Já está em uma corrida!");
            return;
        }

        Debug.Log("[MiniRaceEntryPoint] Iniciando MiniRace!");

        isInRace = true;
        isPlayerNear = false;

        // Salvar posição original do jogador
        if (playerTransform != null)
        {
            playerOriginalPosition = playerTransform.position;
            playerOriginalRotation = playerTransform.rotation;
        }

        // Resetar feedback visual
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // 1. Esconder/desativar o jogador da cidade
        if (hidePlayerDuringRace && playerGameObject != null)
        {
            // Desabilitar CharacterController antes de mover
            if (playerCharacterController != null)
            {
                playerCharacterController.enabled = false;
            }

            // Mover para longe ou desativar
            playerGameObject.SetActive(false);
        }

        // 2. Ativar a área do MiniRace
        if (miniRaceArea != null)
        {
            miniRaceArea.SetActive(true);
        }

        // 3. Ativar o carro
        if (carGameObject != null)
        {
            carGameObject.SetActive(true);

            // Posicionar no ponto de início
            if (raceStartPoint != null)
            {
                carGameObject.transform.position = raceStartPoint.position;
                carGameObject.transform.rotation = raceStartPoint.rotation;

                // Resetar Rigidbody se existir
                Rigidbody carRb = carGameObject.GetComponent<Rigidbody>();
                if (carRb != null)
                {
                    carRb.linearVelocity = Vector3.zero;
                    carRb.angularVelocity = Vector3.zero;
                }
            }

            // Resetar pontuação do carro
            Pontuacao pontuacao = carGameObject.GetComponent<Pontuacao>();
            if (pontuacao == null)
            {
                pontuacao = carGameObject.GetComponentInChildren<Pontuacao>();
            }
            if (pontuacao != null)
            {
                pontuacao.ZerarPontos();
                pontuacao.ZerarCheckpoints();
            }
        }

        // 4. Ativar Canvas/UI
        if (miniRaceCanvas != null)
        {
            miniRaceCanvas.SetActive(true);
        }

        if (cityCanvas != null)
        {
            cityCanvas.SetActive(false);
        }

        // 5. Trocar câmera
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SetMiniRaceCamera();
        }

        // 6. Iniciar sistema de pontuação
        if (raceScoring != null)
        {
            raceScoring.StartRace();
        }

        // 7. Notificar GameManager (se existir)
        if (GameManager.Instance != null)
        {
            // O GameManager pode rastrear que estamos em um minigame
            GameManager.Instance.StartMinigameForQuest("", MinigameType.MiniRace);
        }

        // 8. Disparar evento
        OnRaceStarted?.Invoke();

        Debug.Log("[MiniRaceEntryPoint] MiniRace iniciado com sucesso!");
    }

    /// <summary>
    /// Chamado quando o jogador completa a corrida
    /// </summary>
    public void CompleteMiniRace(int coinsEarned = 0)
    {
        if (!isInRace)
        {
            Debug.LogWarning("[MiniRaceEntryPoint] Não está em uma corrida!");
            return;
        }

        Debug.Log($"[MiniRaceEntryPoint] Corrida completada! Moedas ganhas: {coinsEarned}");

        // 1. Adicionar moedas ao CurrencyManager
        if (coinsEarned > 0 && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinsEarned);
        }

        // 2. Notificar GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMinigameComplete(true, coinsEarned);
        }

        // 3. Sair do minigame
        ExitMiniRace();

        // 4. Disparar eventos
        OnRaceCompleted?.Invoke();
        OnRaceCompletedWithScore?.Invoke(coinsEarned);
    }

    /// <summary>
    /// Cancela/sai do MiniRace
    /// </summary>
    public void ExitMiniRace()
    {
        Debug.Log("[MiniRaceEntryPoint] Saindo do MiniRace...");

        // 1. Desativar carro e área
        if (carGameObject != null)
        {
            carGameObject.SetActive(false);
        }

        if (miniRaceArea != null)
        {
            miniRaceArea.SetActive(false);
        }

        if (miniRaceCanvas != null)
        {
            miniRaceCanvas.SetActive(false);
        }

        if (cityCanvas != null)
        {
            cityCanvas.SetActive(true);
        }

        // 2. Restaurar jogador
        if (hidePlayerDuringRace && playerGameObject != null)
        {
            playerGameObject.SetActive(true);

            // Teleportar para o ponto de retorno
            Transform returnPoint = cityReturnPoint != null ? cityReturnPoint : transform;
            
            playerTransform.position = returnPoint.position;
            playerTransform.rotation = returnPoint.rotation;

            Physics.SyncTransforms();

            // Reativar CharacterController
            if (playerCharacterController != null)
            {
                playerCharacterController.enabled = true;
            }
        }

        // 3. Restaurar câmera da cidade
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SetCityCamera();
        }

        // 4. Marcar que saiu da corrida
        isInRace = false;

        Debug.Log("[MiniRaceEntryPoint] Jogador voltou à cidade!");
    }

    /// <summary>
    /// Reseta o estado do ponto de entrada
    /// </summary>
    public void ResetEntryPoint()
    {
        isInRace = false;
        isPlayerNear = false;

        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    // Getters
    public bool IsInRace() => isInRace;
    public bool IsPlayerNear() => isPlayerNear;
}
