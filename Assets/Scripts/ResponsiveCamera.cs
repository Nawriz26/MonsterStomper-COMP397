using UnityEngine;

/// <summary>
/// Adjusts the camera's vertical FOV to maintain a minimum horizontal FOV
/// on ultra-wide screens, preventing the world from appearing cropped.
/// </summary>
[RequireComponent(typeof(Camera))]
public class ResponsiveCamera : MonoBehaviour
{
    [Tooltip("The target horizontal field of view at the reference aspect ratio.")]
    [SerializeField] private float targetHorizontalFOV = 90f;

    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    void Update()
    {
        // Convert target horizontal FOV to vertical FOV for current aspect ratio
        float hFovRad = targetHorizontalFOV * Mathf.Deg2Rad;
        float vFovRad = 2f * Mathf.Atan(Mathf.Tan(hFovRad / 2f) / cam.aspect);
        cam.fieldOfView = vFovRad * Mathf.Rad2Deg;
    }
}
