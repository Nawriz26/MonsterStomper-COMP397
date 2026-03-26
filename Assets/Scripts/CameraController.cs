using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float height = 5f;
    [SerializeField] private float followSpeed = 10f;

    [Header("Mouse Look")]
    [SerializeField] private bool enableMouseLook = true;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    private float currentX = 0f;
    private float currentY = 20f;

    // Set by MobileTouchController each frame when touch look is active
    private Vector2 externalLookDelta = Vector2.zero;
    private bool hasExternalLookInput = false;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (hasExternalLookInput)
            HandleExternalLook();
        else if (enableMouseLook)
            HandleMouseLook();

        hasExternalLookInput = false;

        UpdateCameraPosition();
    }

    private void HandleMouseLook()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseDelta = mouse.delta.ReadValue();
        currentX += mouseDelta.x * mouseSensitivity;
        currentY -= mouseDelta.y * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
    }

    private void HandleExternalLook()
    {
        currentX += externalLookDelta.x;
        currentY -= externalLookDelta.y;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    /// <summary>
    /// Called by MobileTouchController each frame to drive camera rotation from touch swipe.
    /// sensitivity and invertY are already applied by the caller.
    /// </summary>
    public void AddLookDelta(Vector2 delta)
    {
        externalLookDelta = delta;
        hasExternalLookInput = true;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
