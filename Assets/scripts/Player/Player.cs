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
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineCamera freeLookVCam;
    [SerializeField] private InputReader input;
    [SerializeField] private Transform mainCam;
    [SerializeField] private CheckPoints checkPoints;
    [SerializeField] private Animator BlackoutAnimator;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem runParticles;
    [SerializeField] private ParticleSystem meowParticles;
    [SerializeField] private PlayerSensitivity sensitivity;

    [Header("Movement Settings")]
    [SerializeField] private float constMoveSpeed = 300f;
    [SerializeField] private float constRotationSpeed = 720f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float sprintSpeed = 2.0f;

    private float moveSpeed;
    private float rotationSpeed = 360f;
    private bool isSprinting = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpDuration = 0.25f;
    [SerializeField] private float jumpMaxHeight = 2.5f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField] private float coyoteTime = 0.2f;

    private float jumpCooldown = 0f;
    private float coyoteTimeCounter = 0f;
    private bool wasGroundedLastFrame = false;
    private bool inTheAir = false;

    [Header("CheckPoints")]
    [SerializeField] private int checkPointIndex = 0;

    [Header("Misc")]
    public PlayerNumber playerNumber;
    [SerializeField] private float pickupCooldown = 0.5f;
    private float pickupCooldownTimer = 0f;

    private const float ZERO_F = 0f;
    private const float MIN_MOVEMENT_THRESHOLD = 0.01f;
    private const float IMPACT_VELOCITY_THRESHOLD = 2f;

    private float currentSpeed;
    private float velocity;
    private float jumpVelocity;

    [HideInInspector] public Vector3 movement;

    private List<Timer> timers;
    private CountdownTimer jumpTimer;
    private CountdownTimer jumpCooldownTimer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public AudioClip moewSound;
    public float stepInterval = 0.5f;

    private float stepTimer = 0f;

    [HideInInspector] public bool isRestricted = false;

    private Mechaniki mechaniki;
    public Hats hats;

    // Cached components and values
    private ParticleSystem.EmissionModule runParticlesEmission;

    // Animator hash IDs for better performance
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsSprintingHash = Animator.StringToHash("isSprinting");
    private static readonly int InTheAirHash = Animator.StringToHash("inTheAir");
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    private static readonly int BlackOutHash = Animator.StringToHash("BlackOut");

    public void Die()
    {
        checkPoints.ResetToCheckPoint(transform, checkPointIndex);
        rb.linearVelocity = Vector3.zero;
        BlackoutAnimator.SetTrigger(BlackOutHash);
    }

    void Awake()
    {
        InitializeCamera();
        InitializePhysics();
        InitializeTimers();
        RegisterWithGameManager();
        InitializeComponents();
        InitializeSettings();
    }

    private void InitializeCamera()
    {
        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);
    }

    private void InitializePhysics()
    {
        rb.freezeRotation = true;
    }

    private void InitializeTimers()
    {
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };

        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
    }

    private void RegisterWithGameManager()
    {
        if (GameManager.Instance == null) return;

        switch (playerNumber)
        {
            case PlayerNumber.First:
                GameManager.Instance.player1 = this;
                break;
            case PlayerNumber.Second:
                GameManager.Instance.player2 = this;
                break;
        }
    }

    private void InitializeComponents()
    {
        hats = GetComponentInChildren<Hats>();
        mechaniki = GetComponent<Mechaniki>();
        runParticlesEmission = runParticles.emission;
    }

    private void InitializeSettings()
    {
        SetSensitivity();
    }
    public void SetSensitivity()
    {
        sensitivity.SetJoyStickSensitivity();
        sensitivity.SetMouseSensitivity();
    }

    void Start()
    {
        input.EnablePlayerActions();
        moveSpeed = constMoveSpeed;
        rotationSpeed = constRotationSpeed;
        PauseMenu.Instance.isBusy = false;

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

    void OnDisable()
    {
        input.Jump -= OnJump;
        input.Sprint -= OnSprint;
        input.Moew -= OnSound;
        input.Pause -= OnPause;
        input.PickUp -= OnPickUp;
    }

    private void OnPickUp()
    {
        if (pickupCooldownTimer <= 0)
        {
            mechaniki.PickUpObject();
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

    void OnJump(bool performed)
    {
        bool canJump = !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning &&
                      (groundChecker.IsGrounded || coyoteTimeCounter > 0);

        if (performed && canJump)
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
        HandleInput();
        HandleSprintEffects();
        HandleCoyoteTime();
        UpdateCooldowns();
        HandleTimers();
        UpdateAnimator();
    }

    private void HandleInput()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

        if (isSprinting)
        {
            movement *= sprintSpeed;
        }
    }

    private void HandleSprintEffects()
    {
        bool shouldShowSprintEffects = isSprinting && groundChecker.IsGrounded;

        runParticlesEmission.enabled = shouldShowSprintEffects;
        animator.SetBool(IsSprintingHash, shouldShowSprintEffects);
    }

    private void HandleCoyoteTime()
    {
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
    }

    private void UpdateCooldowns()
    {
        if (pickupCooldownTimer > 0)
        {
            pickupCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        HandleJump();
        HandleMovement();
    }

    void LateUpdate()
    {
        HandleGroundState();
    }

    private void HandleGroundState()
    {
        bool isGrounded = groundChecker.IsGrounded;
        animator.SetBool(InTheAirHash, !isGrounded);

        if (isGrounded && inTheAir)
        {
            inTheAir = false;
        }
        else if (!isGrounded)
        {
            inTheAir = true;
        }
    }

    void UpdateAnimator()
    {
        bool isMoving = movement.sqrMagnitude > MIN_MOVEMENT_THRESHOLD;
        animator.SetBool(IsWalkingHash, isMoving);
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
        if (isRestricted) return;

        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZERO_F;
            animator.SetBool(InTheAirHash, false);
            animator.SetBool(IsJumpingHash, false);
            return;
        }

        if (jumpTimer.IsRunning)
        {
            animator.SetBool(IsJumpingHash, true);
            animator.SetBool(InTheAirHash, true);
            jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
        }
        else if (jumpVelocity >= maxFallSpeed)
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }

    public void SetJumpVelocity(float value)
    {
        rb.AddForce(Vector3.up * value, ForceMode.VelocityChange);
        jumpVelocity = value;
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
        if (collision.relativeVelocity.magnitude > IMPACT_VELOCITY_THRESHOLD && groundChecker.IsGrounded)
        {
            PlaySound(footstepSound);
        }
    }

    void HandleMovement()
    {
        Vector3 adjustedDirection = CalculateMovementDirection();

        if (adjustedDirection.magnitude > ZERO_F)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
            HandleFootsteps();
        }
        else
        {
            SmoothSpeed(ZERO_F);
            rb.linearVelocity = new Vector3(ZERO_F, rb.linearVelocity.y, ZERO_F);
        }
    }

    private Vector3 CalculateMovementDirection()
    {
        if (isRestricted)
        {
            return transform.forward * movement.z;
        }

        return Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
    }

    private void HandleFootsteps()
    {
        float stepMultiplier = isSprinting ? 1.25f : 1f;
        stepTimer -= Time.deltaTime * stepMultiplier;

        if (stepTimer <= 0f && groundChecker.IsGrounded)
        {
            PlaySound(footstepSound);
            stepTimer = stepInterval;
        }
    }

    void HandleHorizontalMovement(Vector3 adjustedDirection)
    {
        Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void HandleRotation(Vector3 adjustedDirection)
    {
        if (isRestricted || adjustedDirection.sqrMagnitude < MIN_MOVEMENT_THRESHOLD)
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

    public void DisableRb(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }
}