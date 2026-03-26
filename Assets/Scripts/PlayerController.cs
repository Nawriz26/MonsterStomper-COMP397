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
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;

    private Vector2 moveInput;
    private bool jumpInput;

    private PlayerHealth playerHealth;
    private PlayerWeapon playerWeapon;
    private PlayerInput playerInput;
    private PauseMenuController pauseMenuController;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerInput = GetComponent<PlayerInput>();
        
        // Find pause menu controller (works even if inactive)
        pauseMenuController = FindObjectOfType<PauseMenuController>(true);
    }

    void Start()
    {
        if (groundCheck == null)
            Debug.LogWarning("PlayerController: GroundCheck is not assigned. Assign the GroundCheck child GameObject in the Inspector.");

        SetupInputCallbacks();
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

    void OnDestroy()
    {
        if (playerInput == null) return;

        var gameplayMap = playerInput.actions.FindActionMap("Gameplay");
        if (gameplayMap == null) return;

        var moveAction = gameplayMap.FindAction("Move");
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
        }

        var jumpAction = gameplayMap.FindAction("Jump");
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
        }

        var fireAction = gameplayMap.FindAction("Fire");
        if (fireAction != null)
        {
            fireAction.performed -= OnFire;
        }

        var pauseAction = gameplayMap.FindAction("Pause");
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
        }
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
        if (moveInput.magnitude < 0.01f) return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
            
            jumpInput = false;
        }
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
