using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadeAndar = 4.0f;
    public float velocidadeCorrer = 8.0f;
    public float alturaPulo = 1.5f;
    public float gravidade = -9.81f;
    public float suavidadeRotacao = 0.1f;

    [Header("Referências")]
    public CharacterController character;
    public Animator animator;
    public Transform cameraTransform;

    private Vector3 velocidadeY;
    private float velocidadeGiroAtual;

    void Start()
    {
        if (!character) character = GetComponent<CharacterController>();
        if (!animator) animator = GetComponent<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 🚫 TRAVA TOTAL SE O MINIGAME NÃO COMEÇOU
        if (CollectClaroGameManager.Instance == null ||
            CollectClaroGameManager.Instance.estadoAtual != EstadoCollectClaro.Jogando)
            return;

        // --- 1. Velocidade (Shift para correr) ---
        bool correndo = Input.GetKey(KeyCode.LeftShift);
        float velocidadeAtual = correndo ? velocidadeCorrer : velocidadeAndar;

        // --- 2. Input ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // --- 3. ANIMAÇÃO (CORREÇÃO APLICADA AQUI) ---
        // 🔁 Inversão do eixo Z para alinhar animação com movimento
        float zAnimacao = -z;

        float multiplicadorAnimacao = correndo ? 2f : 1f;
        animator.SetFloat("InputX", x * multiplicadorAnimacao, 0.1f, Time.deltaTime);
        animator.SetFloat("InputZ", zAnimacao * multiplicadorAnimacao, 0.1f, Time.deltaTime);

        // --- 4. Direção e rotação ---
        Vector3 direcao = new Vector3(x, 0f, z).normalized;
        bool estaNoChao = character.isGrounded;

        if (estaNoChao && velocidadeY.y < 0)
            velocidadeY.y = -2f;

        if (direcao.magnitude >= 0.1f)
        {
            float anguloAlvo =
                Mathf.Atan2(direcao.x, direcao.z) * Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            float angulo = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                anguloAlvo,
                ref velocidadeGiroAtual,
                suavidadeRotacao
            );

            transform.rotation = Quaternion.Euler(0f, angulo, 0f);

            Vector3 moverPara =
                Quaternion.Euler(0f, anguloAlvo, 0f) * Vector3.forward;

            character.Move(moverPara.normalized * velocidadeAtual * Time.deltaTime);
        }

        // --- 5. Pulo ---
        if (Input.GetButtonDown("Jump") && estaNoChao)
        {
            velocidadeY.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
            animator.SetTrigger("pular");
        }

        // --- 6. Gravidade ---
        velocidadeY.y += gravidade * Time.deltaTime;
        character.Move(velocidadeY * Time.deltaTime);
    }
}
