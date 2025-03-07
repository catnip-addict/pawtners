using System;
using UnityEditor;
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
    [SerializeField] private float fallMultiplier = 2.5f;
    public float rotationSpeed = 45f;
    [SerializeField] private PlayerControllerType playerController;
    [SerializeField] private Animator animator;
    PlayerControllerType lastPlayerController;
    [SerializeField] Renderer playerRenderer;
    [SerializeField] Transform hatPosition;
    [SerializeField] CapsuleCollider capsulecollider;

    private Rigidbody rb;
    private bool isGrounded;
    private Input_Actions inputActions;
    public Vector2 moveInput; //sorki Macieju musia�em :((
    private Vector2 cameraInput;
    private float jumpChargeTime;
    private float weight;
    private bool isChargingJump;
    private GameObject playerHat;
    [SerializeField] private LayerMask groundLayer;

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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float radius = capsulecollider.radius * 1.5f;
        Vector3 pos = transform.position + Vector3.down * (radius * 1.5f);
        // DrawWireCapsule(pos, pos + Vector3.up * (radius * 3), radius);
        Gizmos.DrawWireSphere(pos, radius);
    }
    void FixedUpdate()
    {
        if (isGrounded)
        {
            Movment();
        }
        if (rb.linearVelocity.y < 0) // Sprawdzamy, czy gracz spada
        {
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
        }
    }
    void Update()
    {
        float radius = capsulecollider.radius * 1.5f;
        Vector3 pos = transform.position + Vector3.down * (radius * 1.5f);
        isGrounded = Physics.CheckSphere(pos, radius, groundLayer);

        if (playerController != lastPlayerController)
        {
            OnEnable();
            lastPlayerController = playerController;
        }

        if (isChargingJump)
        {
            jumpChargeTime += Time.deltaTime;
            jumpChargeTime = Mathf.Clamp(jumpChargeTime, 0f, maxChargeTime);
        }

        // Rotation based on horizontal input
        float rotationAmount = moveInput.x * rotationSpeed;
        transform.Rotate(0, rotationAmount * Time.deltaTime, 0);
    }

    void Movment()
    {
        // Forward movement based on vertical input only
        Vector3 move = transform.forward * moveInput.y;
        move = move.normalized * (speed - weight);
        rb.linearVelocity = new(move.x, rb.linearVelocity.y, move.z);

        if (moveInput.sqrMagnitude > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isChargingJump)
        {
            isChargingJump = true;
            jumpChargeTime = 0f;
        }
        else if (context.canceled && isGrounded)
        {
            isChargingJump = false;
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTime / maxChargeTime);
            Debug.Log(jumpForce);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    // private void OnCollisionStay(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }
    public void DrawWireCapsule(Vector3 point1, Vector3 point2, float radius)
    {
        Vector3 upOffset = point2 - point1;
        Vector3 up = upOffset.Equals(default) ? Vector3.up : upOffset.normalized;
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, up);
        Vector3 forward = orientation * Vector3.forward;
        Vector3 right = orientation * Vector3.right;
        // z axis
        Handles.DrawWireArc(point2, forward, right, 180, radius);
        Handles.DrawWireArc(point1, forward, right, -180, radius);
        Handles.DrawLine(point1 + right * radius, point2 + right * radius);
        Handles.DrawLine(point1 - right * radius, point2 - right * radius);
        // x axis
        Handles.DrawWireArc(point2, right, forward, -180, radius);
        Handles.DrawWireArc(point1, right, forward, 180, radius);
        Handles.DrawLine(point1 + forward * radius, point2 + forward * radius);
        Handles.DrawLine(point1 - forward * radius, point2 - forward * radius);
        // y axis
        Handles.DrawWireDisc(point2, up, radius);
        Handles.DrawWireDisc(point1, up, radius);
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