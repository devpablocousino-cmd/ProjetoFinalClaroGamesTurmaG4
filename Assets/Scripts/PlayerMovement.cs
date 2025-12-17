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

    private Vector2 keyboardInput;
    private Vector2 uiInput;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        ReadKeyboardInput();
    }

    // =======================
    // MOVIMENTO
    // =======================
    void Move()
    {
        moveInput = Vector2.ClampMagnitude(keyboardInput + uiInput, 1f);

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

    // =======================
    // INPUT (UI / MOBILE)
    // =======================

    public void SetMoveX(float value)
    {
        uiInput.x = value;
    }

    public void SetMoveY(float value)
    {
        uiInput.y = value;
    }

    public void StopMoveX()
    {
        uiInput.x = 0f;
    }

    public void StopMoveY()
    {
        uiInput.y = 0f;
    }

    public void SetSprint(bool value)
    {
        sprintPressed = value;
    }

    public void PressJump()
    {
        jumpPressed = true;
    }

    void ReadKeyboardInput()
    {
    var keyboard = Keyboard.current;
    if (keyboard == null)
    {
        keyboardInput = Vector2.zero;
        return;
    }

    float x = 0f;
    float y = 0f;

    if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x = -1;
    else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x = 1;

    if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y = 1;
    else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y = -1;

    keyboardInput = new Vector2(x, y);

    sprintPressed = keyboard.shiftKey.isPressed;

    if (keyboard.spaceKey.wasPressedThisFrame)
        jumpPressed = true;
        // Interação (E)
        if (keyboard.eKey.wasPressedThisFrame)
        {
            var interaction = GetComponent<PlayerInteraction>();
            if (interaction != null)
                interaction.PressInteract();
        }
    }
    

}
