using UnityEngine;

/// <summary>
/// Handles player weapon firing.
/// Bullets are aimed toward the camera crosshair using a screen-centre raycast,
/// so they always travel where the player is looking regardless of body rotation.
/// Uses ProjectileFactory (Factory Method pattern) when assigned,
/// otherwise falls back to legacy direct Instantiate.
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    [Header("Factory (Factory Method Pattern)")]
    [Tooltip("Assign a GameObject that has a ProjectileFactory component.")]
    [SerializeField] private ProjectileFactory projectileFactory;

    [Header("Fallback — Legacy Direct Spawn (used only if no factory assigned)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private int damage = 25;

    [Header("Weapon Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;

    [Header("Aim")]
    [Tooltip("How far the screen-centre raycast travels to find a target point. " +
             "Bullets always fly toward this point from the fire point.")]
    [SerializeField] private float aimRange = 100f;
    [Tooltip("Layers the aim raycast hits. Leave empty to hit everything.")]
    [SerializeField] private LayerMask aimLayers = ~0;

    private Camera mainCamera;
    private float nextFireTime = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        if (firePoint == null)
        {
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0, 1f, 1f);
            firePoint = fp.transform;
        }
    }

    /// <summary>Fires a projectile aimed at the camera crosshair.</summary>
    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        FireProjectile();
        nextFireTime = Time.time + fireRate;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayShoot();
    }

    private void FireProjectile()
    {
        // ── Determine aim direction from camera screen centre ─────────────────
        Vector3 aimPoint = GetAimPoint();
        Vector3 aimDirection = (aimPoint - firePoint.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);

        if (projectileFactory != null)
        {
            GameObject proj = projectileFactory.Create(firePoint.position, aimRotation);

            // Override velocity to match aim direction (factory sets rotation but
            // ProjectileFactory.Create already applies forward velocity from the rotation)
            if (proj != null)
            {
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float speed = GetConfigSpeed();
                    rb.linearVelocity = aimDirection * speed;
                }
            }
        }
        else if (projectilePrefab != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, aimRotation);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = aimDirection * projectileSpeed;

            Projectile ps = proj.GetComponent<Projectile>();
            if (ps != null)
                ps.Initialize(damage);

            Destroy(proj, 5f);
        }
        else
        {
            Debug.LogWarning("PlayerWeapon: assign either a ProjectileFactory or a fallback projectilePrefab.");
        }
    }

    /// <summary>
    /// Casts a ray from the centre of the screen along the camera forward axis.
    /// Returns the hit point if something is hit, otherwise returns a point
    /// <see cref="aimRange"/> units ahead of the camera.
    /// </summary>
    private Vector3 GetAimPoint()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, aimRange, aimLayers))
            return hit.point;

        return ray.origin + ray.direction * aimRange;
    }

    /// <summary>Reads speed from the factory's config if available.</summary>
    private float GetConfigSpeed()
    {
        if (projectileFactory != null)
            return projectileFactory.Speed;

        return projectileSpeed;
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 3f);
        }
    }
}
