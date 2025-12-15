using UnityEngine;
using UnityEngine.InputSystem;

public class MazeCameraInput : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform playerTransform;    // O jogador que a câmera deve seguir
    [SerializeField] private Vector3 shoulderOffset = new Vector3(0.5f, 1.6f, 0f); // Offset do ombro (altura dos olhos)

    [Header("Camera Settings")]
    [SerializeField] private float defaultDistance = 3f;   // Distância padrão da câmera ao jogador
    [SerializeField] private float minDistance = 0.5f;     // Distância mínima (quando colidir)
    [SerializeField] private float maxDistance = 5f;       // Distância máxima

    [Header("Rotation Settings")]
    [SerializeField] private float sensitivityX = 150f;
    [SerializeField] private float sensitivityY = 150f;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float minPitch = -40f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayers;    // Layers que a câmera deve evitar (paredes, etc.)
    [SerializeField] private float collisionRadius = 0.2f; // Raio da esfera de colisão
    [SerializeField] private float collisionSmoothing = 10f; // Suavização ao colidir

    private float yaw;      // rotação em Y (esquerda/direita)
    private float pitch;    // rotação em X (cima/baixo)
    private float currentDistance;
    private float targetDistance;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        // Reinicializa quando a câmera for ativada
        isInitialized = false;
        Initialize();
        Debug.Log("[MazeCameraInput] Câmera do labirinto ATIVADA");
    }

    private void Initialize()
    {
        // Inicializa a distância
        currentDistance = defaultDistance;
        targetDistance = defaultDistance;

        // Se não tiver player configurado, tenta encontrar
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log($"[MazeCameraInput] Player encontrado: {player.name}");
            }
            else
            {
                Debug.LogError("[MazeCameraInput] ERRO: Player não encontrado! Certifique-se que o jogador tem a tag 'Player'");
                return;
            }
        }

        // Inicializa yaw baseado na rotação do player
        if (playerTransform != null)
        {
            yaw = playerTransform.eulerAngles.y;
            Debug.Log($"[MazeCameraInput] Inicializado - Player pos: {playerTransform.position}, Yaw: {yaw}");
        }
        pitch = 10f; // Ângulo inicial levemente para baixo
        isInitialized = true;
    }

    void LateUpdate()
    {
        if (!isInitialized || playerTransform == null)
        {
            // Tenta encontrar novamente
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Initialize();
            }
            else
            {
                return;
            }
        }

        HandleRotationInput();
        HandleCameraPosition();
    }

    private void HandleRotationInput()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * (invertX ? -1f : 1f);
        float mouseY = mouseDelta.y * (invertY ? -1f : 1f);

        yaw += mouseX * sensitivityX * Time.deltaTime;
        pitch -= mouseY * sensitivityY * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleCameraPosition()
    {
        // Ponto de origem (ombro/cabeça do jogador)
        Vector3 targetPosition = playerTransform.position + playerTransform.TransformDirection(shoulderOffset);

        // Calcula a direção da câmera baseada na rotação
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direction = rotation * Vector3.back; // Direção oposta (atrás do jogador)

        // Verifica colisão com paredes
        targetDistance = defaultDistance;
        RaycastHit hit;

        if (Physics.SphereCast(targetPosition, collisionRadius, direction, out hit, maxDistance, collisionLayers))
        {
            // Se colidir, ajusta a distância
            targetDistance = Mathf.Clamp(hit.distance - collisionRadius, minDistance, maxDistance);
        }

        // Suaviza a transição de distância
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * collisionSmoothing);

        // Posição final da câmera
        Vector3 cameraPosition = targetPosition + direction * currentDistance;

        // Aplica posição e rotação
        transform.position = cameraPosition;
        transform.rotation = rotation;
    }

    /// <summary>
    /// Define a rotação inicial da câmera (útil ao entrar no labirinto)
    /// </summary>
    public void SetInitialRotation(float initialYaw, float initialPitch = 10f)
    {
        yaw = initialYaw;
        pitch = initialPitch;
    }

    /// <summary>
    /// Reseta a câmera para seguir o player atual
    /// </summary>
    public void ResetToPlayer()
    {
        if (playerTransform != null)
        {
            yaw = playerTransform.eulerAngles.y;
            pitch = 10f;
            currentDistance = defaultDistance;
        }
    }
}
