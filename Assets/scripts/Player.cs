
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

public class Player : MonoBehaviour
{
    //
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineCamera freeLookVCam;
    [SerializeField] InputReader input;
    [SerializeField] Transform mainCam;
    [SerializeField] CheckPoints checkPoints;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 300f;
    [SerializeField] float rotationSpeed = 360f;
    [SerializeField] float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 60f;
    [SerializeField] float jumpDuration = 0.25f;
    float jumpCooldown = 0f;
    [SerializeField] float jumpMaxHeight = 2.5f;
    [SerializeField] float gravityMultiplier = 2f;
    [Header("CheckPoints")]
    [SerializeField] int checkPointIndex = 0;

    const float ZeroF = 0f;
    float currentSpeed;
    float velocity;
    float jumpVelocity;

    Vector3 movement;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;

    public void Die()
    {
        // Reset player position
        checkPoints.ResetToCheckPoint(transform, checkPointIndex);
        // Reset player velocity
        rb.linearVelocity = Vector3.zero;
    }

    void Awake()
    {

        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
        freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

        rb.freezeRotation = true;

        // Setup timers
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        timers = new(2) { jumpTimer, jumpCooldownTimer };

        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
    }

    void Start() => input.EnablePlayerActions();

    void OnEnable()
    {
        input.Jump += OnJump;
    }

    void OnDisable()
    {
        input.Jump -= OnJump;
    }

    void OnJump(bool performed)
    {
        if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpTimer.Start();
        }
        else if (!performed && jumpTimer.IsRunning)
        {
            jumpTimer.Stop();
        }
    }

    void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

        HandleTimers();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
    }

    void UpdateAnimator()
    {
        // animator.SetFloat(Speed, currentSpeed);
        if (movement.sqrMagnitude > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    void HandleJump()
    {
        // If not jumping and grounded, keep jump velocity at 0
        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZeroF;
            return;
        }

        // If jumping or falling calculate velocity
        if (jumpTimer.IsRunning)
        {
            jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
        }
        else
        {
            // Gravity takes over
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }

        // Apply velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }

    void HandleMovement()
    {
        // Rotate movement direction to match camera rotation
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);

            // Reset horizontal velocity for a snappy stop
            rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
        }
    }

    void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        // Move the player
        Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;

        // Debug.Log(velocity);

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void HandleRotation(Vector3 adjustedDirection)
    {
        // Adjust rotation to match movement direction
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
