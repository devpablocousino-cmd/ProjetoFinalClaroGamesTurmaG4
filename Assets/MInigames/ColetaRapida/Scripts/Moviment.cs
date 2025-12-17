using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Moviment : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeAndar = 4f;
    public float velocidadeCorrer = 7f;
    public float suavidadeRotacao = 10f;

    [Header("Pulo")]
    public float alturaPulo = 1.5f;
    public float gravidade = -20f;

    [Header("Referências")]
    public CharacterController character;
    public Animator animator;
    public Transform cameraTransform;

    // Estados internos
    private Vector3 velocidadeVertical;
    private bool estaPulando;
    private bool iniciou;

    // Animator (nomes EXATOS)
    private const string PARAM_X = "InputX";
    private const string PARAM_Z = "InputZ";
    private const string PARAM_GROUNDED = "Grounded";
    private const string TRIGGER_PULAR = "pular";

    void Start()
    {
        if (!character) character = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;

        // Corrige bugs clássicos
        animator.ResetTrigger(TRIGGER_PULAR);
        velocidadeVertical = Vector3.zero;
        estaPulando = false;
        iniciou = false;
    }

    void Update()
    {
        // Ignora o primeiro frame (CharacterController ainda não sabe se está no chão)
        if (!iniciou)
        {
            iniciou = true;
            return;
        }

        bool estaNoChao = character.isGrounded;

        if (estaNoChao && velocidadeVertical.y < 0f)
        {
            velocidadeVertical.y = -2f;
            if (estaPulando) estaPulando = false;
        }

        // ================= INPUT =================
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector2 inputPlano = new Vector2(x, z);
        float intensidade = Mathf.Clamp01(inputPlano.magnitude);

        bool correndo = Input.GetKey(KeyCode.LeftShift);
        float velocidadeAtual = correndo ? velocidadeCorrer : velocidadeAndar;

        // ================= DIREÇÃO RELATIVA À CÂMERA =================
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 direcaoMovimento = camForward * z + camRight * x;

        // ================= ROTAÇÃO DO PLAYER (BASE DA CÂMERA) =================
        if (direcaoMovimento.sqrMagnitude > 0.001f)
        {
            Quaternion alvoRotacao = Quaternion.LookRotation(direcaoMovimento);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                alvoRotacao,
                suavidadeRotacao * Time.deltaTime
            );
        }

        // ================= MOVIMENTO HORIZONTAL (SEM PATINAÇÃO) =================
        Vector3 movimentoHorizontal =
            direcaoMovimento.normalized *
            velocidadeAtual *
            intensidade;

        // ================= PULO =================
        if (Input.GetButtonDown("Jump") && estaNoChao && !estaPulando)
        {
            estaPulando = true;
            velocidadeVertical.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
            animator.SetTrigger(TRIGGER_PULAR);
        }

        // ================= GRAVIDADE =================
        velocidadeVertical.y += gravidade * Time.deltaTime;

        // ================= MOVE FINAL =================
        Vector3 movimentoFinal =
            movimentoHorizontal +
            velocidadeVertical;

        character.Move(movimentoFinal * Time.deltaTime);

        // ================= ANIMATOR =================
        animator.SetBool(PARAM_GROUNDED, estaNoChao);

        if (!estaPulando)
        {
            float multAnim = correndo ? 2f : 1f;

            // 🔥 CORREÇÃO DE EIXO: animações usam Z invertido
            animator.SetFloat(PARAM_X, x * multAnim, 0.1f, Time.deltaTime);
            animator.SetFloat(PARAM_Z, -z * multAnim, 0.1f, Time.deltaTime);
        }
        else
        {
            // Evita locomotion cancelar o pulo
            animator.SetFloat(PARAM_X, 0f);
            animator.SetFloat(PARAM_Z, 0f);
        }
    }
}
