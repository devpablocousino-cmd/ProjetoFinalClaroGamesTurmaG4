using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    private Animator animator;
    [SerializeField] private Transform foot; //objeto responsavel pelo chao
    [SerializeField] private LayerMask colisionLayer; //layer de colisao

    [Header("Variables")]
    public float velocity = 5f; //qual a forca de movimentacao do personagem
    public float sprintMultiplier = 1.5f; // multiplicador de velocidade ao pressionar Shift
    private bool isGround; //verifica se o personagem esta no chao
    private float yForce; //forca aplicada no eixo y 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Retorna a câmera ativa atual (atualiza dinamicamente quando troca de câmera)
    /// </summary>
    private Transform GetActiveCamera()
    {
        Camera mainCam = Camera.main;
        return mainCam != null ? mainCam.transform : null;
    }

    void Update()
    {
        Move();
        Jump();
    }

    public void Move()
    {
        //Debug.Log("Executando o movimento do personagem...");

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;

        Vector3 movimento = new Vector3(horizontal, 0, vertical);

        movimento = Vector3.ClampMagnitude(movimento, 1f); // Normaliza a velocidade diagonal de movimento
        
        // Usa a câmera ativa atual para calcular a direção do movimento
        Transform activeCamera = GetActiveCamera();
        if (activeCamera != null)
        {
            movimento = activeCamera.TransformDirection(movimento);
        }
        movimento.y = 0; // Mant�m o movimento no plano horizontal

        // Detecta se o jogador est� correndo (Shift esquerdo ou direito)
        bool isSprinting = false;
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            isSprinting = keyboard.shiftKey.isPressed;
        }

        float currentSpeed = velocity * (isSprinting ? sprintMultiplier : 1f);

        controller.Move(movimento * currentSpeed * Time.deltaTime); // Aplica o movimento ao CharacterController 
        if (movimento != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(movimento),
                Time.deltaTime * 10f
             );
        }

        animator.SetBool("Mover", movimento != Vector3.zero);
        // Se quiser adicionar uma anima��o de corrida, crie um par�metro "Run" no Animator e descomente a linha abaixo:
        animator.SetBool("Run", isSprinting && movimento != Vector3.zero);

        isGround = Physics.CheckSphere(foot.position, 0.3f, colisionLayer);
        //Botar parametro isGround no animator
        animator.SetBool("isGround", isGround);
    }

    public void Jump()
    {
        //Debug.Log("Estou no ch�o?" + isGround);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGround)
        {
            yForce = 5f;
            animator.SetTrigger("Jump");
        }

        if (yForce > -9.81f)
        {
            yForce += -9.81f * Time.deltaTime;
        }

        controller.Move(new Vector3(0, yForce, 0) * Time.deltaTime); // Aplica a gravidade ao CharacterController

    }

}