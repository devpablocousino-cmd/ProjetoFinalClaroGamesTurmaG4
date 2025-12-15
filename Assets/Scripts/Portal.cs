using UnityEngine;
using System.Collections;

/// <summary>
/// Teleporta o Player de um portal para outro (bidirecional).
/// Coloque em cada porta e configure o "receiver" apontando para o outro portal.
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Portal Configuration")]
    public Transform receiver;     // Arraste o objeto "destination" aqui no Inspector
    public bool usaCooldown = true;
    public float cooldown = 0.5f;

    [Header("Exit Placement")]
    [Tooltip("Offset aplicado na SAÍDA em coordenadas locais do receiver. Use Z>0 para sair para frente da porta.")]
    [SerializeField] private Vector3 receiverLocalOffset = new Vector3(0f, 0f, 1f);
    [Tooltip("Se true, o Player sai com a rotação do receiver (útil para orientar na direção correta).")]
    [SerializeField] private bool matchReceiverRotation = true;

    // Cooldown compartilhado estático para evitar ping-pong entre portais
    private static bool globalCooldownActive = false;
    private static float globalCooldownEndTime = 0f;

    // Referência ao CharacterController para garantir reativação mesmo se coroutine for interrompida
    private CharacterController cachedCharacterController;

    private void Start()
    {
        // Validação inicial
        if (receiver == null)
        {
            Debug.LogError($"[{gameObject.name}] ERRO: Receiver não está configurado no Inspector!");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Portal configurado. Receiver: {receiver.name}");
        }

        // Verifica se tem Collider e se está configurado como Trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[{gameObject.name}] ERRO: Nenhum Collider encontrado!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"[{gameObject.name}] AVISO: Collider não está marcado como 'Is Trigger'!");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Collider configurado corretamente como Trigger");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{gameObject.name}] OnTriggerEnter detectado! Objeto: {other.gameObject.name}, Tag: {other.tag}");

        // Usar cooldown global para evitar ping-pong entre portais
        if (globalCooldownActive && Time.time < globalCooldownEndTime)
        {
            Debug.Log($"[{gameObject.name}] Teleporte bloqueado (cooldown global ativo)");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log($"[{gameObject.name}] Objeto não é Player. Tag atual: {other.tag}");
            return;
        }

        if (receiver == null)
        {
            Debug.LogError($"[{gameObject.name}] Receiver não configurado!");
            return;
        }

        Debug.Log($"[{gameObject.name}] Iniciando teleporte...");
        Teleportar(other);
    }

    /// <summary>
    /// Teleporte síncrono (sem coroutine) para evitar problemas se o objeto for desativado.
    /// </summary>
    private void Teleportar(Collider other)
    {
        // Ativar cooldown global imediatamente
        globalCooldownActive = true;
        globalCooldownEndTime = Time.time + cooldown;

        Debug.Log($"[{gameObject.name}] Cooldown global ativado por {cooldown}s");

        // Salva posição antiga para debug
        Vector3 oldPosition = other.transform.position;

        // Pega e cacheia o CharacterController
        CharacterController characterController = other.GetComponent<CharacterController>();
        cachedCharacterController = characterController;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        // Se tem CharacterController, precisa desabilitar temporariamente
        if (characterController != null)
        {
            characterController.enabled = false;
            Debug.Log($"[{gameObject.name}] CharacterController desabilitado temporariamente");
        }

        // Se tem Rigidbody, zera a velocidade
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log($"[{gameObject.name}] Velocidade do Rigidbody zerada");
        }

        // Calcula posição e rotação de destino
        Vector3 targetPosition = receiver.position + receiver.TransformDirection(receiverLocalOffset);
        Quaternion targetRotation = matchReceiverRotation ? receiver.rotation : other.transform.rotation;

        // Teletransporta
        other.transform.SetPositionAndRotation(targetPosition, targetRotation);
        Physics.SyncTransforms();

        Debug.Log($"[{gameObject.name}] ✓ TELEPORTADO de {oldPosition} para {targetPosition} ({receiver.name})");

        // Reabilita CharacterController IMEDIATAMENTE (não espera frame)
        if (characterController != null)
        {
            characterController.enabled = true;
            cachedCharacterController = null;
            Debug.Log($"[{gameObject.name}] CharacterController reabilitado");
        }
    }

    /// <summary>
    /// Garante que o CharacterController seja reabilitado mesmo se algo der errado.
    /// </summary>
    private void OnDisable()
    {
        // Segurança: se o portal for desativado durante teleporte, reabilita o CharacterController
        if (cachedCharacterController != null)
        {
            cachedCharacterController.enabled = true;
            Debug.LogWarning($"[{gameObject.name}] OnDisable: CharacterController forçadamente reabilitado!");
            cachedCharacterController = null;
        }
    }

    /// <summary>
    /// Reseta o cooldown global (útil para debug ou ao reiniciar o jogo).
    /// </summary>
    public static void ResetGlobalCooldown()
    {
        globalCooldownActive = false;
        globalCooldownEndTime = 0f;
        Debug.Log("[Portal] Cooldown global resetado");
    }
}
