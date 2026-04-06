using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f;
    [Tooltip("Multiplier applied to gravity while the player is falling (velocity.y < 0). " +
             "Values above 1 make landing faster and snappier.")]
    [SerializeField] private float fallGravityMultiplier = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;

    private Vector2 moveInput;
    private bool jumpInput;

    private PlayerHealth playerHealth;
    private PlayerWeapon playerWeapon;
    private PlayerInput playerInput;
    private PauseMenuController pauseMenuController;

    // Animator parameter IDs — cached to avoid per-frame string hashing
    private static readonly int AnimBlend  = Animator.StringToHash("Blend");
    private static readonly int AnimJump   = Animator.StringToHash("Jump");
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimDeath  = Animator.StringToHash("Death");

    void Awake()
    {
        controller  = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerInput  = GetComponent<PlayerInput>();

        // Animator lives on the Hero_Rock visual child, not on the root
        animator = GetComponentInChildren<Animator>();

        // Find pause menu controller (works even if inactive)
        pauseMenuController = FindObjectOfType<PauseMenuController>(true);
    }

    void Start()
    {
        // Auto-resolve GroundCheck by name so it works even when the
        // Inspector reference is lost after a scene restore.
        if (groundCheck == null)
        {
            Transform found = transform.Find("GroundCheck");
            if (found != null)
                groundCheck = found;
            else
                Debug.LogWarning("PlayerController: GroundCheck child not found. Add a child GameObject named 'GroundCheck'.");
        }

        // Subscribe to death event to trigger the Death animation
        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(OnPlayerDeath);

        SetupInputCallbacks();
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(OnPlayerDeath);

        // Unsubscribe input callbacks below
        UnsubscribeInputCallbacks();
    }

    /// <summary>Plays the Death animation when the player dies.</summary>
    private void OnPlayerDeath()
    {
        if (animator != null)
            animator.SetTrigger(AnimDeath);
    }

    private void SetupInputCallbacks()
    {
        if (playerInput == null) return;

        var gameplayMap = playerInput.actions.FindActionMap("Gameplay");
        if (gameplayMap == null) return;

        var moveAction = gameplayMap.FindAction("Move");
        if (moveAction != null)
        {
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }

        var jumpAction = gameplayMap.FindAction("Jump");
        if (jumpAction != null)
        {
            jumpAction.performed += OnJump;
        }

        var sprintAction = gameplayMap.FindAction("Sprint");
        if (sprintAction != null)
        {
            sprintAction.performed += ctx => isSprinting = true;
            sprintAction.canceled += ctx => isSprinting = false;
        }

        var fireAction = gameplayMap.FindAction("Fire");
        if (fireAction != null)
        {
            fireAction.performed += OnFire;
        }

        var pauseAction = gameplayMap.FindAction("Pause");
        if (pauseAction != null)
        {
            pauseAction.performed += OnPause;
        }
    }

    private void UnsubscribeInputCallbacks()
    {
        if (playerInput == null) return;

        var gameplayMap = playerInput.actions.FindActionMap("Gameplay");
        if (gameplayMap == null) return;

        var moveAction = gameplayMap.FindAction("Move");
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled  -= OnMove;
        }

        var jumpAction = gameplayMap.FindAction("Jump");
        if (jumpAction != null)
            jumpAction.performed -= OnJump;

        var fireAction = gameplayMap.FindAction("Fire");
        if (fireAction != null)
            fireAction.performed -= OnFire;

        var pauseAction = gameplayMap.FindAction("Pause");
        if (pauseAction != null)
            pauseAction.performed -= OnPause;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        CheckGrounded();
        ApplyGravity();
        HandleMovement();
        HandleJump();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void ApplyGravity()
    {
        // Apply stronger gravity multiplier while falling so the arc feels snappy
        // and the player doesn't float down slowly after the peak
        float gravityScale = velocity.y < 0f ? fallGravityMultiplier : 1f;
        velocity.y += gravity * gravityScale * Time.deltaTime;

        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (move.sqrMagnitude > 0.001f)
        {
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            controller.Move(move * currentSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Drive the Idle↔Run blend tree: 0 = idle, 1 = running
        if (animator != null)
        {
            float blendValue = move.sqrMagnitude > 0.001f ? 1f : 0f;
            animator.SetFloat(AnimBlend, blendValue, 0.1f, Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            // Trigger the Jump animation
            if (animator != null)
                animator.SetTrigger(AnimJump);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJump();

            jumpInput = false;
        }
    }

    /// <summary>
    /// Detects when the player lands on top of an enemy (stomp).
    /// A stomp is valid when the player is falling (velocity.y &lt; 0)
    /// and the contact normal points upward — meaning the player hit the top of the enemy.
    /// </summary>
    private const float StompMinNormalY = 0.5f;
    private const float StompBounceForce = 8f;
    private const int StompDamage = 999;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Only stomp while falling and hitting a surface that faces upward
        if (velocity.y >= 0f) return;
        if (hit.normal.y < StompMinNormalY) return;

        EnemyHealth enemy = hit.gameObject.GetComponent<EnemyHealth>();
        if (enemy == null)
            enemy = hit.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemy == null) return;

        enemy.TakeDamage(StompDamage);

        // Bounce the player upward so the stomp feels responsive
        velocity.y = StompBounceForce;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && playerWeapon != null)
        {
            playerWeapon.Fire();

            // Trigger the Attack animation
            if (animator != null)
                animator.SetTrigger(AnimAttack);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If not cached yet, try to find it now
            if (pauseMenuController == null)
            {
                pauseMenuController = FindObjectOfType<PauseMenuController>(true);
            }

            if (pauseMenuController != null)
            {
                pauseMenuController.TogglePause();
            }
            else
            {
                Debug.LogWarning("PauseMenuController not found! Make sure there's a GameObject with PauseMenuController in the scene.");
            }
        }
    }

    // ── Mobile input API — called by MobileTouchController ────────────

    /// <summary>Sets movement direction from the virtual joystick each frame.</summary>
    public void SetMobileMove(Vector2 input) => moveInput = input;

    /// <summary>Sets sprint state from the mobile sprint button.</summary>
    public void SetMobileSprint(bool sprinting) => isSprinting = sprinting;

    /// <summary>Triggers a jump from the mobile jump button.</summary>
    public void TriggerMobileJump() => jumpInput = true;

    /// <summary>Called by StompDetector to bounce the player upward after killing an enemy.</summary>
    public void ApplyStompBounce(float force)
    {
        velocity.y = Mathf.Sqrt(force * -2f * gravity);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
