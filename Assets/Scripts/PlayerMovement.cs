using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoviment : MonoBehaviour
{
    [Header("Configurações")]
    public float velocidade = 6f;
    public float alturaPulo = 1.5f;
    public float gravidade = -9.81f;

    [Header("Referências")]
    [SerializeField] private Transform peDoPersonagem; // Crie um objeto vazio na sola do pé
    [SerializeField] private LayerMask camadaChao;     // O que é considerado chão?

    // Variáveis internas
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // Controla a gravidade (Y)
    private bool isGrounded;
    private Vector2 inputMovimento;
    private bool botaoPulo;
    private bool botaoCorrer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Trava o mouse no centro da tela (opcional, bom para testar)
        // Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        LerInputs();
        AplicarGravidade(); // O SEGREDO ESTÁ AQUI
        Mover();
    }

    void LerInputs()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Leitura Simples e Direta
        float x = 0;
        float z = 0;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1;
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) z = 1;
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) z = -1;

        inputMovimento = new Vector2(x, z);
        botaoCorrer = keyboard.shiftKey.isPressed;

        // Só permite pular se apertou Espaço neste frame exato
        if (keyboard.spaceKey.wasPressedThisFrame) botaoPulo = true;
    }

    void AplicarGravidade()
    {
        // 1. Verifica se está no chão (crie uma esfera pequena no pé)
        isGrounded = Physics.CheckSphere(peDoPersonagem.position, 0.2f, camadaChao);

        if (animator) animator.SetBool("isGround", isGrounded);

        // 2. CORREÇÃO DO "DEFICIENTE":
        // Se já está no chão, paramos de empurrar ele para baixo infinitamente.
        // Deixamos apenas -2f para garantir que ele fique colado no piso.
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 3. Pulo
        if (botaoPulo && isGrounded)
        {
            // Fórmula da física: v = raiz(altura * -2 * gravidade)
            velocity.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
            if (animator) animator.SetTrigger("Jump");
        }
        botaoPulo = false; // Reseta o botão

        // 4. Aplica a gravidade constante
        velocity.y += gravidade * Time.deltaTime;

        // 5. Move o eixo Y (Cair ou Pular)
        controller.Move(velocity * Time.deltaTime);
    }

    void Mover()
    {
        Vector3 direcao = new Vector3(inputMovimento.x, 0, inputMovimento.y).normalized;

        // Se tem movimento
        if (direcao.magnitude >= 0.1f)
        {
            // Rotação: Calcula o ângulo baseado na câmera
            float anguloAlvo = Mathf.Atan2(direcao.x, direcao.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            // Suaviza a rotação
            transform.rotation = Quaternion.Euler(0f, anguloAlvo, 0f);

            // Calcula a direção para frente baseada na rotação
            Vector3 direcaoMove = Quaternion.Euler(0f, anguloAlvo, 0f) * Vector3.forward;

            // Define velocidade (correr ou andar)
            float velAtual = velocidade * (botaoCorrer ? 1.5f : 1f);

            // Move o Character Controller
            controller.Move(direcaoMove.normalized * velAtual * Time.deltaTime);
        }

        // Animação
        if (animator)
        {
            bool andando = direcao.magnitude >= 0.1f;
            animator.SetBool("Mover", andando);
            animator.SetBool("Run", andando && botaoCorrer);
        }
    }
}