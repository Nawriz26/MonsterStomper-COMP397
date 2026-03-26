using UnityEngine;

/// <summary>
/// Factory Method implementation for projectile creation.
/// Instantiates a projectile prefab and applies ProjectileConfig stats.
/// Used by PlayerWeapon instead of direct Instantiate calls.
/// </summary>
public class ProjectileFactory : MonoBehaviour, IGameObjectFactory
{
    [Header("Prefab")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Configuration")]
    [SerializeField] private ProjectileConfig config;

    /// <summary>The launch speed defined by this factory's config.</summary>
    public float Speed => config != null ? config.speed : 20f;

    /// <summary>Creates a configured projectile at the given position and rotation.</summary>
    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectileFactory: projectilePrefab is not assigned.");
            return null;
        }

        GameObject projectile = Instantiate(projectilePrefab, position, rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = rotation * Vector3.forward * (config != null ? config.speed : 20f);

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(config != null ? config.damage : 25);

        float lifetime = config != null ? config.lifetime : 5f;
        Destroy(projectile, lifetime);

        return projectile;
    }
}
