using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum PlayerControllerType
    {
        Keyboard,
        Gamepad
    }

    [SerializeField] public float speed = 5f;
    [SerializeField] public float maxJumpForce = 10f;
    [SerializeField] public float minJumpForce = 5f;
    [SerializeField] public float maxChargeTime = 1f;
    [SerializeField] public float rotationSpeed = 45f;
    [SerializeField] public PlayerControllerType playerController;
    PlayerControllerType lastPlayerController;


    private Rigidbody rb;
    private bool isGrounded;
    private Input_Actions inputActions;
    private Vector2 moveInput;
    private Vector2 cameraInput;
    private float jumpChargeTime;
    private bool isChargingJump;

    void Awake()
    {
        inputActions = new Input_Actions();
        lastPlayerController = playerController;
    }

    void OnEnable()
    {
        if (playerController == PlayerControllerType.Keyboard)
        {
            inputActions.Keyboard.Chodzenie.performed += OnChodzenie;
            inputActions.Keyboard.Chodzenie.canceled += OnChodzenie;
            inputActions.Keyboard.Kamera.performed += OnKamera;
            inputActions.Keyboard.Kamera.canceled += OnKamera;
            inputActions.Keyboard.Skok.performed += OnSkok;
            inputActions.Keyboard.Skok.canceled += OnSkok;
            inputActions.Keyboard.Enable();
            ///
            inputActions.Controller.Chodzenie.performed -= OnChodzenie;
            inputActions.Controller.Chodzenie.canceled -= OnChodzenie;
            inputActions.Controller.Kamera.performed -= OnKamera;
            inputActions.Controller.Kamera.canceled -= OnKamera;
            inputActions.Controller.Skok.performed -= OnSkok;
            inputActions.Controller.Skok.canceled -= OnSkok;
            inputActions.Controller.Disable();
        }
        else if (playerController == PlayerControllerType.Gamepad)
        {
            inputActions.Controller.Chodzenie.performed += OnChodzenie;
            inputActions.Controller.Chodzenie.canceled += OnChodzenie;
            inputActions.Controller.Kamera.performed += OnKamera;
            inputActions.Controller.Kamera.canceled += OnKamera;
            inputActions.Controller.Skok.performed += OnSkok;
            inputActions.Controller.Skok.canceled += OnSkok;
            inputActions.Controller.Enable();
            //
            inputActions.Keyboard.Chodzenie.performed -= OnChodzenie;
            inputActions.Keyboard.Chodzenie.canceled -= OnChodzenie;
            inputActions.Keyboard.Kamera.performed -= OnKamera;
            inputActions.Keyboard.Kamera.canceled -= OnKamera;
            inputActions.Keyboard.Skok.performed -= OnSkok;
            inputActions.Keyboard.Skok.canceled -= OnSkok;
            inputActions.Keyboard.Disable();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isGrounded)
        {

            Move();
            Rotate();
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

    void Move()
    {

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * speed;
        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVelocity;
    }

    void Rotate()
    {

        float rot = cameraInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rot, 0);
    }

    void OnChodzenie(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnSkok(InputAction.CallbackContext context)
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

    void OnKamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }
    void OnJoin(InputAction.CallbackContext context)
    {
        Debug.Log("fun");
    }

    private void OnCollisionEnter(Collision collision)
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
}