using UnityEngine;

/// <summary>
/// Singleton pool for player projectiles.
/// Replaces Instantiate / Destroy in PlayerWeapon with pool Get / Release calls,
/// eliminating per-shot GC allocations on mobile.
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int initialPoolSize = 20;

    private ObjectPool<Projectile> pool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        pool = new ObjectPool<Projectile>(projectilePrefab, initialPoolSize, transform);
    }

    /// <summary>Gets a projectile from the pool, positions it, and initialises damage.</summary>
    public Projectile Get(Vector3 position, Quaternion rotation, int damage, float speed, Vector3 direction)
    {
        Projectile proj = pool.Get();
        proj.transform.SetPositionAndRotation(position, rotation);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = direction * speed;
        }

        proj.Initialize(damage, this);
        return proj;
    }

    /// <summary>Returns a projectile to the pool instead of destroying it.</summary>
    public void Release(Projectile proj)
    {
        pool.Release(proj);
    }
}
