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
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;

        // Cria o vetor de movimento relativo ao input
        Vector3 movimento = new Vector3(horizontal, 0, vertical);

        // Normaliza para evitar movimento mais rápido na diagonal (magnitude máxima 1f)
        movimento = Vector3.ClampMagnitude(movimento, 1f);

        // Transforma o input relativo à direção que a câmera está olhando (orientação da câmera)
        movimento = myCamera.TransformDirection(movimento);
        movimento.y = 0; // Mantém o movimento estritamente no plano horizontal (sem voar)

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