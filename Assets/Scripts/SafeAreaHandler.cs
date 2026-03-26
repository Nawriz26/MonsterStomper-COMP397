using UnityEngine;

/// <summary>
/// Adjusts a Canvas RectTransform to stay within the device safe area.
/// Attach to the root panel directly beneath the Canvas.
/// Handles notches, Dynamic Island, home bars, and gesture nav bars on both Android and iOS.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2 lastScreenSize = Vector2.zero;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // Reapply if the screen or safe area changed (rotation, split-screen, etc.)
        Vector2 currentSize = new Vector2(Screen.width, Screen.height);
        if (Screen.safeArea != lastSafeArea || currentSize != lastScreenSize)
            ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        if (safeArea == lastSafeArea &&
            new Vector2(Screen.width, Screen.height) == lastScreenSize)
            return;

        lastSafeArea = safeArea;
        lastScreenSize = new Vector2(Screen.width, Screen.height);

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
