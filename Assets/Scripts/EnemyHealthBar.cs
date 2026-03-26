using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space health bar that floats above an enemy and always faces the camera.
/// Drives the fill by interpolating fillRect.anchorMax.x directly — works with
/// any Image type without requiring Image.type = Filled.
/// Attach to a Canvas (World Space) child of the enemy GameObject.
/// Subscribes to EnemyHealth.OnHealthChanged automatically.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color fullColor = new Color(0.18f, 0.75f, 0.18f);
    [SerializeField] private Color lowColor  = new Color(0.85f, 0.15f, 0.15f);

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, 0f);

    private Transform enemyTransform;
    private Camera mainCamera;

    void Awake()
    {
        enemyTransform = transform.parent;
        mainCamera = Camera.main;

        // Auto-resolve fill references from children if not assigned in Inspector
        if (fillRect == null)
        {
            Transform fill = transform.Find("Fill");
            if (fill != null) fillRect = fill.GetComponent<RectTransform>();
        }

        if (fillImage == null && fillRect != null)
            fillImage = fillRect.GetComponent<Image>();

        EnemyHealth health = GetComponentInParent<EnemyHealth>();
        if (health != null)
            health.OnHealthChanged.AddListener(OnHealthChanged);
    }

    void LateUpdate()
    {
        if (enemyTransform == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        transform.position = enemyTransform.position + offset;

        transform.LookAt(
            transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    /// <summary>Called by EnemyHealth.OnHealthChanged event.</summary>
    public void OnHealthChanged(int current, int max)
    {
        float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
        SetFill(ratio);
    }

    private void SetFill(float ratio)
    {
        // Scale the fill bar by moving anchorMax.x — no Filled image type needed
        if (fillRect != null)
        {
            Vector2 anchorMax = fillRect.anchorMax;
            anchorMax.x = ratio;
            fillRect.anchorMax = anchorMax;
        }

        if (fillImage != null)
            fillImage.color = Color.Lerp(lowColor, fullColor, ratio);
    }
}
