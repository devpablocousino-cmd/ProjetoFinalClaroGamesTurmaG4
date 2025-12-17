using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    private Animator animator;

    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask colisionLayer;

    [Header("Movement")]
    public float velocity = 5f;
    public float sprintMultiplier = 1.5f;

    private Vector2 moveInput;
    private bool sprintPressed;
    private bool jumpPressed;

    private bool isGround;
    private float yForce;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    // =======================
    // INPUT SYSTEM CALLBACKS
    // =======================

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        sprintPressed = value.isPressed;
    }

    void OnJump()
    {
        jumpPressed = true;
    }

    void OnInteract()
    {
        var interaction = GetComponent<PlayerInteraction>();
        if (interaction != null)
            interaction.PressInteract();
    }

    // =======================
    // MOVIMENTO
    // =======================
    void Move()
    {
        Vector3 movimento = new Vector3(moveInput.x, 0, moveInput.y);
        movimento = Vector3.ClampMagnitude(movimento, 1f);

        Transform cam = Camera.main?.transform;
        if (cam != null && movimento != Vector3.zero)
        {
            float yaw = cam.eulerAngles.y;
            movimento = Quaternion.Euler(0, yaw, 0) * movimento;
        }

        float speed = velocity * (sprintPressed ? sprintMultiplier : 1f);
        controller.Move(movimento * speed * Time.deltaTime);

        if (movimento != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movimento),
                Time.deltaTime * 10f
            );
        }

        animator.SetBool("Mover", movimento != Vector3.zero);
        animator.SetBool("Run", sprintPressed && movimento != Vector3.zero);

        isGround = Physics.CheckSphere(foot.position, 0.3f, colisionLayer);
        animator.SetBool("isGround", isGround);
    }

    // =======================
    // PULO
    // =======================
    void Jump()
    {
        if (jumpPressed && isGround)
        {
            yForce = 5f;
            animator.SetTrigger("Jump");
        }

        jumpPressed = false;

        yForce += Physics.gravity.y * Time.deltaTime;
        controller.Move(Vector3.up * yForce * Time.deltaTime);
    }
}
