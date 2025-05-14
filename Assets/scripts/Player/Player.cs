using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

public enum PlayerNumber
{
    First,
    Second,
    Both,
    Whatever
}

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
    [SerializeField] Animator BlackoutAnimator;
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] ParticleSystem runParticles;
    [SerializeField] ParticleSystem meowParticles;

    [Header("Movement Settings")]
    [SerializeField] float constMoveSpeed = 300f;
    float moveSpeed;
    [SerializeField] float constRotationSpeed = 720f;
    float rotationSpeed = 360f;
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] float sprintSpeed = 2.0f;
    bool isSprinting = false;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 60f;
    [SerializeField] float jumpDuration = 0.25f;
    float jumpCooldown = 0f;
    [SerializeField] float jumpMaxHeight = 2.5f;
    [SerializeField] float gravityMultiplier = 2f;
    [SerializeField] float maxFallSpeed = 10f;
    [SerializeField] float coyoteTime = 0.2f;
    private float coyoteTimeCounter = 0f;
    private bool wasGroundedLastFrame = false;
    private bool inTheAir = false;

    [Header("CheckPoints")]
    [SerializeField] int checkPointIndex = 0;
    [Header("Misc")]
    public PlayerNumber playerNumber;
    [SerializeField] private float pickupCooldown = 0.5f; // Cooldown time in seconds for pickup action
    private float pickupCooldownTimer = 0f;

    const float ZeroF = 0f;
    float currentSpeed;
    float velocity;
    float jumpVelocity;

    [HideInInspector]
    public Vector3 movement;

    List<Timer> timers;
    CountdownTimer jumpTimer;
    CountdownTimer jumpCooldownTimer;
    [Header("Audio st00pek")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public AudioClip moewSound;
    public float stepInterval = 0.5f;

    private float stepTimer = 0f;
    [HideInInspector]
    public bool isRestricted = false;
    CinemachineInputAxisController inputAxisController;
    Mechaniki mechaniki;
    public Hats hats;

    public void Die()
    {
        checkPoints.ResetToCheckPoint(transform, checkPointIndex);
        rb.linearVelocity = Vector3.zero;
        BlackoutAnimator.SetTrigger("BlackOut");
    }

    void Awake()
    {
        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

        rb.freezeRotation = true;

        // Setup timers
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        timers = new(2) { jumpTimer, jumpCooldownTimer };

        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

        float mouseSens = PlayerPrefs.GetFloat("MouseSens", 1.0f);
        SetMouseSensitivity(mouseSens);
        hats = GetComponentInChildren<Hats>();
        mechaniki = GetComponent<Mechaniki>();
    }

    public void SetMouseSensitivity(float mouseSens)
    {
        var axisControllers = freeLookVCam.GetComponents<CinemachineInputAxisController>();

        foreach (var axisController in axisControllers)
        {
            foreach (var c in axisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                {
                    c.Input.LegacyGain = 1f * mouseSens;
                    c.Input.Gain = 2f * mouseSens;
                    // Debug.Log($"Setting X sensitivity on controller: {axisController.name}");
                }
                if (c.Name == "Look Orbit Y")
                {
                    c.Input.LegacyGain = -1f * mouseSens;
                    c.Input.Gain = -2f * mouseSens;
                    // Debug.Log($"Setting Y sensitivity on controller: {axisController.name}");
                }
            }
        }
    }

    void Start()
    {
        input.EnablePlayerActions();
        moveSpeed = constMoveSpeed;
        rotationSpeed = constRotationSpeed;
        PauseMenu.Instance.isBusy = false;
        if (GameManager.Instance != null)
        {
            if (playerNumber == PlayerNumber.First)
            {
                GameManager.Instance.player1 = this;
            }
            else if (playerNumber == PlayerNumber.Second)
            {
                GameManager.Instance.player2 = this;
            }
        }
        int hatIndex = PlayerPrefs.GetInt("HatIndex" + playerNumber, 0);
        if (hatIndex != 0)
        {
            AchievementManager.Instance.UnlockAchievement(12);
        }
    }
    void OnEnable()
    {
        input.Jump += OnJump;
        input.Moew += OnSound;
        input.Sprint += OnSprint;
        input.Pause += OnPause;
        input.PickUp += OnPickUp;
    }

    private void OnPickUp()
    {
        // Only allow pickup/drop if not on cooldown
        if (pickupCooldownTimer <= 0)
        {
            mechaniki.PickUpObject();
            // Reset cooldown timer
            pickupCooldownTimer = pickupCooldown;
        }
    }

    private void OnPause()
    {
        PauseMenu.Instance.CheckForPause();
    }

    private void OnSound(bool pressed)
    {
        if (pressed)
        {
            PlaySound(moewSound);
            meowParticles.Play();
        }
    }

    void OnDisable()
    {
        input.Jump -= OnJump;
        input.Sprint -= OnSprint;
        input.Moew -= OnSound;
        input.Pause -= OnPause;
        input.PickUp -= OnPickUp;
    }

    void OnJump(bool performed)
    {
        if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && (groundChecker.IsGrounded || coyoteTimeCounter > 0))
        {
            jumpTimer.Start();
            JumpParticles();
            coyoteTimeCounter = 0f;
        }
        else if (!performed && jumpTimer.IsRunning)
        {
            jumpTimer.Stop();
        }
    }

    void OnSprint(bool sprinting)
    {
        isSprinting = sprinting;
    }

    void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        var emission = runParticles.emission;
        if (isSprinting)
        {
            movement *= sprintSpeed;
            if (groundChecker.IsGrounded)
            {
                emission.enabled = true;
                animator.SetBool("isSprinting", true);
            }
            else
            {
                emission.enabled = false;
                animator.SetBool("isSprinting", false);
            }
        }
        else
        {
            emission.enabled = false;
            animator.SetBool("isSprinting", false);
        }

        if (groundChecker.IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else if (wasGroundedLastFrame && !groundChecker.IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        wasGroundedLastFrame = groundChecker.IsGrounded;

        if (pickupCooldownTimer > 0)
        {
            pickupCooldownTimer -= Time.deltaTime;
        }

        HandleTimers();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
    }
    void LateUpdate()
    {
        if (groundChecker.IsGrounded)
        {
            animator.SetBool("inTheAir", false);
            if (inTheAir)
            {
                // CinemachineShakeManager.Instance.Shake(freeLookVCam, 0.5f, 0.15f);
                inTheAir = false;
            }
        }
        else
        {
            inTheAir = true;
            animator.SetBool("inTheAir", true);
        }
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

    public void SetWeight(float weight)
    {
        moveSpeed = constMoveSpeed - weight * 50;
        rotationSpeed = constRotationSpeed - weight;
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
        if (isRestricted)
            return;

        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZeroF;
            animator.SetBool("inTheAir", false);
            animator.SetBool("isJumping", false);
            return;
        }

        if (jumpTimer.IsRunning)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("inTheAir", true);

            jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
        }
        else
        {
            if (jumpVelocity >= maxFallSpeed)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }
    public void SetJumpVelocity(float value)
    {
        rb.AddForce(Vector3.up * value, ForceMode.VelocityChange);
        jumpVelocity = value;
        Debug.Log("Jump velocity set to: " + jumpVelocity);
    }
    public void JumpParticles()
    {
        jumpParticles.Play();
    }
    public Mechaniki GetMechaniki()
    {
        return mechaniki;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f && groundChecker.IsGrounded)
        {
            PlaySound(footstepSound);
        }
    }

    void HandleMovement()
    {
        _ = Vector3.zero;
        Vector3 adjustedDirection;
        if (isRestricted)
        {
            adjustedDirection = transform.forward * movement.z;
        }
        else
        {
            adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
        }

        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);

            if (isSprinting)
            {
                stepTimer -= Time.deltaTime * 1.25f;
            }
            else
            {
                stepTimer -= Time.deltaTime;
            }
            if (stepTimer <= 0f && groundChecker.IsGrounded)
            {
                PlaySound(footstepSound);
                stepTimer = stepInterval;
            }
        }

        else
        {
            SmoothSpeed(ZeroF);
            rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
        }
    }

    void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void HandleRotation(Vector3 adjustedDirection)
    {
        if (isRestricted)
            return;

        if (adjustedDirection.sqrMagnitude < 0.01f)
            return;

        var targetRotation = Quaternion.LookRotation(adjustedDirection);

        float rotationFactor = rotationSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactor);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
    public void PlaySound(AudioClip clip)
    {
        float originalPitch = audioSource.pitch;
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(clip);
        audioSource.pitch = originalPitch;
    }
    public void TeleportToPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void ZeroEverything()
    {
        rb.linearDamping = 0f;
        jumpVelocity = 0f;
        rb.angularVelocity = Vector3.zero;
    }
    public void DisableRb(bool what)
    {
        rb.isKinematic = what;
    }
    public void SetPlayerHat(int index)
    {
        throw new NotImplementedException();
    }

    public void SetPlayerMat(Material material)
    {
        throw new NotImplementedException();
    }
}

