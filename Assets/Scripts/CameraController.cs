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

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (enableMouseLook)
        {
            HandleMouseLook();
        }

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

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
