using UnityEngine;
using UnityEngine.InputSystem;

public class NieWiemJeszcze : MonoBehaviour, Input_Actions.IWalkingActions
{
    
    [SerializeField] public float speed = 5f; 
    [SerializeField] public float maxJumpForce = 10f;
    [SerializeField] public float minJumpForce = 5f; 
    [SerializeField] public float maxChargeTime = 1f; 
    [SerializeField] public float rotationSpeed = 45f; 

    
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
        
        inputActions.Walking.SetCallbacks(this);
    }

    void OnEnable()
    {
        
        inputActions.Walking.Enable();
    }

    void OnDisable()
    {
        
        inputActions.Walking.Disable();
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
    }

    void Move()
    {
        
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = move.normalized * speed;
        Vector3 newVelocity = new Vector3(move.x, rb.velocity.y, move.z);
        rb.velocity = newVelocity;
    }

    void Rotate()
    {

        float rot = cameraInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rot, 0);
    }

    public void OnChodzenie(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSkok(InputAction.CallbackContext context)
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

    public void OnKamera(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
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