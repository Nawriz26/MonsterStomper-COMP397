using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space health bar that floats above an enemy and always faces the camera.
/// Attach to a Canvas (World Space) child of the enemy GameObject.
/// Subscribe to EnemyHealth.OnHealthChanged to update the fill.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color lowColor = Color.red;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);

    private Transform enemyTransform;
    private Camera mainCamera;

    void Awake()
    {
        enemyTransform = transform.parent;
        mainCamera = Camera.main;

        EnemyHealth health = GetComponentInParent<EnemyHealth>();
        if (health != null)
        {
            health.OnHealthChanged.AddListener(OnHealthChanged);
        }
    }

    void LateUpdate()
    {
        if (enemyTransform == null || mainCamera == null) return;

        // Follow enemy with vertical offset
        transform.position = enemyTransform.position + offset;

        // Always face the main camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    /// <summary>Called by EnemyHealth.OnHealthChanged event.</summary>
    public void OnHealthChanged(int current, int max)
    {
        if (healthSlider == null) return;

        float ratio = max > 0 ? (float)current / max : 0f;
        healthSlider.value = ratio;

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowColor, fullColor, ratio);
        }
    }
}
