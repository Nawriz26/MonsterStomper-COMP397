using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Reads virtual on-screen joystick and swipe-to-look input and routes them
/// to PlayerController and CameraController.
///
/// Setup:
///   1. Add this component to the same GameObject as PlayerController.
///   2. Assign the joystick background RectTransform and handle RectTransform.
///   3. Assign the CameraController reference.
///   4. Wire the HUD buttons' EventTrigger/UnityEvent callbacks to the public
///      On*Down / On*Up methods of this script.
///
/// The camera look zone is the right 50% of the screen. Any touch that begins
/// in that region (and not over a UI element) drives camera rotation.
/// </summary>
public class MobileTouchController : MonoBehaviour
{
    // ── References ──────────────────────────────────────────────

    [Header("Joystick UI")]
    [Tooltip("The outer ring / background of the virtual joystick.")]
    [SerializeField] private RectTransform joystickBackground;
    [Tooltip("The inner handle / knob of the virtual joystick.")]
    [SerializeField] private RectTransform joystickHandle;

    [Header("Scene References")]
    [SerializeField] private CameraController cameraController;

    // ── Settings ─────────────────────────────────────────────────

    [Header("Joystick Settings")]
    [Tooltip("Radius in pixels within which the handle can move.")]
    [SerializeField] private float joystickRadius = 80f;
    [Tooltip("Inputs below this normalised magnitude are treated as zero (prevents drift).")]
    [SerializeField] private float deadZone = 0.15f;

    [Header("Camera Swipe Settings")]
    [SerializeField] private float touchSensitivity = 0.15f;
    [SerializeField] private bool invertY = false;

    // ── Runtime state ────────────────────────────────────────────

    private PlayerController playerController;
    private PlayerWeapon playerWeapon;
    private PauseMenuController pauseMenuController;

    private int joystickTouchId = -1;
    private Vector2 joystickOrigin;
    private Vector2 joystickInput;

    private int lookTouchId = -1;
    private Vector2 lastLookPosition;

    // Button state flags (set by UI EventTrigger callbacks)
    private bool jumpPressed;
    private bool sprintHeld;
    private bool fireHeld;

    private float nextFireTime;
    private const float FireRate = 0.2f;

    // ── Lifecycle ────────────────────────────────────────────────

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerWeapon = GetComponent<PlayerWeapon>();

        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();

        pauseMenuController = FindObjectOfType<PauseMenuController>(true);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            ResetJoystick();
            return;
        }

        ProcessTouches();
        SendMovementInput();
        HandleFireHeld();
    }

    // ── Touch processing ─────────────────────────────────────────

    private void ProcessTouches()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        foreach (var touch in touchscreen.touches)
        {
            UnityEngine.InputSystem.TouchPhase phase = touch.phase.ReadValue();
            int id = touch.touchId.ReadValue();
            Vector2 pos = touch.position.ReadValue();

            switch (phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    OnTouchBegan(id, pos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    OnTouchMoved(id, pos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    OnTouchEnded(id);
                    break;
            }
        }
    }

    private void OnTouchBegan(int id, Vector2 pos)
    {
        // Left half of screen → joystick
        if (pos.x < Screen.width * 0.5f)
        {
            if (joystickTouchId == -1)
            {
                joystickTouchId = id;
                joystickOrigin = pos;
                PositionJoystickAt(pos);
                joystickInput = Vector2.zero;
            }
        }
        else
        {
            // Right half → camera look (ignore touches over UI buttons)
            if (lookTouchId == -1 && !IsTouchOverUI(pos))
            {
                lookTouchId = id;
                lastLookPosition = pos;
            }
        }
    }

    private void OnTouchMoved(int id, Vector2 pos)
    {
        if (id == joystickTouchId)
        {
            Vector2 delta = pos - joystickOrigin;
            Vector2 clamped = Vector2.ClampMagnitude(delta, joystickRadius);
            joystickInput = clamped / joystickRadius;

            if (joystickHandle != null)
                joystickHandle.anchoredPosition = clamped;
        }
        else if (id == lookTouchId)
        {
            Vector2 delta = pos - lastLookPosition;
            lastLookPosition = pos;

            float ySign = invertY ? 1f : -1f;
            Vector2 lookDelta = new Vector2(delta.x, delta.y * ySign) * touchSensitivity;

            if (cameraController != null)
                cameraController.AddLookDelta(lookDelta);
        }
    }

    private void OnTouchEnded(int id)
    {
        if (id == joystickTouchId)
        {
            joystickTouchId = -1;
            ResetJoystick();
        }
        else if (id == lookTouchId)
        {
            lookTouchId = -1;
        }
    }

    // ── Input dispatch ────────────────────────────────────────────

    private void SendMovementInput()
    {
        Vector2 input = joystickInput;

        if (input.magnitude < deadZone)
            input = Vector2.zero;

        // Synthesise an InputAction.CallbackContext equivalent by writing directly
        // to the public OnMove entry point on PlayerController
        playerController.SetMobileMove(input);
        playerController.SetMobileSprint(sprintHeld);

        if (jumpPressed)
        {
            playerController.TriggerMobileJump();
            jumpPressed = false;
        }
    }

    private void HandleFireHeld()
    {
        if (fireHeld && Time.time >= nextFireTime)
        {
            playerWeapon?.Fire();
            nextFireTime = Time.time + FireRate;
        }
    }

    private void ResetJoystick()
    {
        joystickInput = Vector2.zero;

        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;

        playerController?.SetMobileMove(Vector2.zero);
    }

    private void PositionJoystickAt(Vector2 screenPos)
    {
        if (joystickBackground == null) return;

        Canvas canvas = joystickBackground.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint);

        joystickBackground.anchoredPosition = localPoint;
        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;
    }

    private bool IsTouchOverUI(Vector2 screenPos)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    // ── Button callbacks — wire these from HUD Button EventTriggers ──

    /// <summary>Called on Jump button PointerDown.</summary>
    public void OnJumpDown()
    {
        jumpPressed = true;
        HapticManager.Instance?.OnJump();
    }

    /// <summary>Called on Sprint button PointerDown.</summary>
    public void OnSprintDown() => sprintHeld = true;

    /// <summary>Called on Sprint button PointerUp.</summary>
    public void OnSprintUp() => sprintHeld = false;

    /// <summary>Called on Fire button PointerDown — starts auto-fire.</summary>
    public void OnFireDown() => fireHeld = true;

    /// <summary>Called on Fire button PointerUp — stops auto-fire.</summary>
    public void OnFireUp() => fireHeld = false;

    /// <summary>Called on Pause button tap.</summary>
    public void OnPauseTap()
    {
        if (pauseMenuController == null)
            pauseMenuController = FindObjectOfType<PauseMenuController>(true);

        pauseMenuController?.TogglePause();
    }

    // ── Settings API — called by OptionsManager ───────────────────

    /// <summary>Updates swipe sensitivity (0.05 – 0.5 recommended).</summary>
    public void SetSensitivity(float value) => touchSensitivity = value;

    /// <summary>Flips the touch look Y axis.</summary>
    public void SetInvertY(bool inverted) => invertY = inverted;
}
