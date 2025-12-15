using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] private Transform mazeEntryPoint;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject mazeAreaGameObject;
    [SerializeField] private GameObject Player;
    [SerializeField] private CharacterController characterController;

    [Header("Scoring")]
    [SerializeField] private TimeBasedScoring timeScoring;

    [Header("Visual Feedback")]
    [SerializeField] private Color highlightColor = Color.yellow;
    private Color originalColor;
    private Renderer meshRenderer;

    private bool isPlayerNear = false;
    private bool isInMaze = false; // Controla se o jogador já está no labirinto

    private void Start()
    {
        // Valida��o
        if (mazeAreaGameObject != null)
        {
            // Garantir que o labirinto est� desativado no in�cio
            mazeAreaGameObject.SetActive(false);
        }

        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;

            if (meshRenderer != null)
            {
                meshRenderer.material.color = highlightColor;
            }

            Debug.Log("[TeleportPoint] Jogador pr�ximo! Pressione E para entrar no labirinto");
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
        }
    }

    private void Update()
    {
        // Só permite entrar no labirinto se estiver perto E não estiver já no labirinto
        if (isPlayerNear && !isInMaze && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EnterMaze();
        }
    }

    private void EnterMaze()
    {
        Debug.Log("[TeleportPoint] Teletransportando para o labirinto!");

        // Marca que o jogador está no labirinto
        isInMaze = true;
        isPlayerNear = false; // Reseta para evitar problemas

        // Resetar feedback visual do portal ao entrar
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        // 1. Ativar a �rea do labirinto
        if (mazeAreaGameObject != null)
        {
            mazeAreaGameObject.SetActive(true);
        }

        // (Re)capturar o TimeBasedScoring mesmo se ele estiver dentro de objetos que começam inativos
        if (timeScoring == null && mazeAreaGameObject != null)
        {
            timeScoring = mazeAreaGameObject.GetComponentInChildren<TimeBasedScoring>(true);
        }

        // Desabilitar o CharacterController antes de teletransportar
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // 2. Teletransportar o jogador
        if (playerTransform != null && mazeEntryPoint != null)
        {
            playerTransform.position = mazeEntryPoint.position;
            playerTransform.rotation = mazeEntryPoint.rotation;

            // Sincroniza as transformações com a física
            Physics.SyncTransforms();

            Debug.Log($"[TeleportPoint] Jogador movido para: {mazeEntryPoint.position}");
        }

        // Reabilitar o CharacterController após teletransportar
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // 3. Trocar a c�mera
        CameraManager.Instance.SetMazeCamera();

        // 4. Resetar a pontua��o do labirinto (opcional)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetMazeScore();
        }

        // 5. Iniciar o timer do bônus por tempo
        if (timeScoring != null)
        {
            timeScoring.StartMazeTimer();
        }
        else
        {
            Debug.LogWarning("[TeleportPoint] TimeBasedScoring não encontrado para iniciar o timer.");
        }
    }

    /// <summary>
    /// Reseta o estado interno do portal para permitir reentrada.
    /// Útil quando a saída do labirinto acontece fora deste script.
    /// </summary>
    public void ResetPortalState()
    {
        isInMaze = false;
        isPlayerNear = false;

        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        Debug.Log("[TeleportPoint] Estado do portal resetado (reentrada permitida)");
    }

    /// <summary>
    /// M�todo p�blico para sair do labirinto (chamado quando o jogador completa o labirinto)
    /// </summary>
    public void ExitMaze(Transform exitPoint)
    {
        Debug.Log("[TeleportPoint] Saindo do labirinto!");

        // Desabilitar o CharacterController antes de teletransportar
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // 1. Teletransportar para a sa�da da cidade
        if (playerTransform != null && exitPoint != null)
        {
            playerTransform.position = exitPoint.position;
            playerTransform.rotation = exitPoint.rotation;

            // Sincroniza as transforma��es com a f�sica
            Physics.SyncTransforms();

            Debug.Log($"[TeleportPoint] Jogador movido para: {exitPoint.position}");
        }

        // Reabilitar o CharacterController ap�s teletransportar
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // 2. Desativar a �rea do labirinto
        if (mazeAreaGameObject != null)
        {
            mazeAreaGameObject.SetActive(false);
        }

        // 3. Restaurar c�mera da cidade
        CameraManager.Instance.SetCityCamera();

        // Marca que o jogador saiu do labirinto (permite entrar novamente)
        isInMaze = false;

        // Não assume que o player está perto do portal ao voltar
        isPlayerNear = false;

        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        Debug.Log("[TeleportPoint] Jogador voltou � cidade!");
    }
}