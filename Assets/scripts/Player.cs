using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum PlayerControllerType
    {
        Keyboard,
        Gamepad
    }

    public float speed = 5f;
    [SerializeField] private float maxJumpForce = 10f;
    [SerializeField] private float minJumpForce = 5f;
    [SerializeField] private float maxChargeTime = 1f;
    public float rotationSpeed = 45f;
    [SerializeField] private PlayerControllerType playerController;
    [SerializeField] private Animator animator;
    PlayerControllerType lastPlayerController;
    [SerializeField] Renderer playerRenderer;
    [SerializeField] Transform hatPosition;

    private Rigidbody rb;
    private bool isGrounded;
    private Input_Actions inputActions;
    public Vector2 moveInput; //sorki Macieju musia�em :((
    private Vector2 cameraInput;
    private float jumpChargeTime;
    private float weight;
    private bool isChargingJump;
    private GameObject playerHat;

    void Awake()
    {
        inputActions = new Input_Actions();
        lastPlayerController = playerController;
    }

    void OnEnable()
    {
        if (playerController == PlayerControllerType.Keyboard)
        {
            // Klawiatura
            inputActions.Keyboard.Walking.performed += OnMove;
            inputActions.Keyboard.Walking.canceled += OnMove;
            inputActions.Keyboard.Look.performed += OnCamera;
            inputActions.Keyboard.Look.canceled += OnCamera;
            inputActions.Keyboard.Jump.performed += OnJump;
            inputActions.Keyboard.Jump.canceled += OnJump;
            inputActions.Keyboard.Enable();
            /// Kontroler
            inputActions.Controller.Walking.performed -= OnMove;
            inputActions.Controller.Walking.canceled -= OnMove;
            inputActions.Controller.Look.performed -= OnCamera;
            inputActions.Controller.Look.canceled -= OnCamera;
            inputActions.Controller.Jump.performed -= OnJump;
            inputActions.Controller.Jump.canceled -= OnJump;
            inputActions.Controller.Disable();
        }
        else if (playerController == PlayerControllerType.Gamepad)
        {
            /// Kontroler
            inputActions.Controller.Walking.performed += OnMove;
            inputActions.Controller.Walking.canceled += OnMove;
            inputActions.Controller.Look.performed += OnCamera;
            inputActions.Controller.Look.canceled += OnCamera;
            inputActions.Controller.Jump.performed += OnJump;
            inputActions.Controller.Jump.canceled += OnJump;
            inputActions.Controller.Enable();
            // Klawiatura
            inputActions.Keyboard.Walking.performed -= OnMove;
            inputActions.Keyboard.Walking.canceled -= OnMove;
            inputActions.Keyboard.Look.performed -= OnCamera;
            inputActions.Keyboard.Look.canceled -= OnCamera;
            inputActions.Keyboard.Jump.performed -= OnJump;
            inputActions.Keyboard.Jump.canceled -= OnJump;
            inputActions.Keyboard.Disable();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isGrounded)
        {
            Movment();
        }

        if (isChargingJump)
        {
            jumpChargeTime += Time.deltaTime;

            jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);
        }
        if (playerController != lastPlayerController)
        {
            OnEnable();
            lastPlayerController = playerController;
        }
    }

    void Movment()
    {
        //movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * (speed - weight);
        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVelocity;
        if (moveInput.magnitude > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        //Rotation
        float rot = cameraInput.x * (rotationSpeed - weight * 10) * Time.deltaTime;
        transform.Rotate(0, rot, 0);
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            isChargingJump = true;
            jumpChargeTime = 0f;
        }
        else if (context.canceled && isGrounded)
        {
            isChargingJump = false;
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


    public void SetWeight(float value)
    {
        weight = value;
    }
    public void SetPlayerMat(Material material)
    {
        playerRenderer.material = material;
    }
    public void SetPlayerHat(GameObject hat)
    {
        if (playerHat != null)
        {
            Destroy(playerHat);
        }
        playerHat = Instantiate(hat, hatPosition);
    }
}