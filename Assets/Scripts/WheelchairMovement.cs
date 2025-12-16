using UnityEngine;
using UnityEngine.InputSystem; // Necessário para Keyboard.current

public class WheelchairMovement : MonoBehaviour
{
    // Variáveis ajustáveis no Inspector
    public float velocity = 5f; // Velocidade de movimento
    public float rotationSmoothness = 10f; // Suavidade da rotação

    // Componentes necessários no objeto
    private CharacterController controller;
    private Transform myCamera; // Referência para a câmera principal do jogador
    [SerializeField] private LayerMask colisionLayer; //layer de colisao

    void Start()
    {
        // Obtém o CharacterController e a Câmera Principal
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController não encontrado! Por favor, adicione um.");
        }

        // Encontra a câmera principal automaticamente
        myCamera = Camera.main.transform;
        if (myCamera == null)
        {
            Debug.LogError("Câmera principal não encontrada! Certifique-se de que sua câmera tenha a tag 'MainCamera'.");
        }
    }

    void Update()
    {
        // A lógica de input e movimento do seu código original é colocada aqui
        Move();
    }

    public void Move()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Captura direta das teclas usando o novo Input System
        // Horizontal: A/Seta Esquerda = -1, D/Seta Direita = +1
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal = 1f;

        // Vertical: W/Seta Cima = +1 (frente), S/Seta Baixo = -1 (trás)
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical = -1f;

        // Cria o vetor de movimento: X = horizontal (esquerda/direita), Z = vertical (frente/trás)
        Vector3 movimento = new Vector3(horizontal, 0, vertical);

        // Normaliza para evitar movimento mais rápido na diagonal (magnitude máxima 1f)
        movimento = Vector3.ClampMagnitude(movimento, 1f);

        // Atualiza a referência da câmera dinamicamente (importante para troca de câmeras)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            myCamera = mainCam.transform;
        }

        // Usa apenas a rotação horizontal (Y) da câmera para calcular a direção
        if (myCamera != null && movimento != Vector3.zero)
        {
            // Pega apenas a rotação Y da câmera (ignora inclinação)
            float cameraYaw = myCamera.eulerAngles.y;
            Quaternion rotacao = Quaternion.Euler(0, cameraYaw, 0);
            movimento = rotacao * movimento;
        }

        // Aplica o movimento usando o método Move() do CharacterController
        // Nota: O CharacterController gerencia automaticamente colisões e gravidade básicas.
        controller.Move(movimento * velocity * Time.deltaTime);

        // Aplica rotação suave do modelo para olhar na direção do movimento
        if (movimento != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movimento),
                Time.deltaTime * rotationSmoothness
             );
        }

        // Removidas as linhas relacionadas ao Animator e Physics.CheckSphere, conforme solicitado.
    }
}