using UnityEngine;

/// <summary>
/// Projectile fired by the player.
/// Supports both legacy Destroy and Object Pool release patterns.
/// When spawned via ProjectilePool, it returns itself to the pool instead of destroying.
/// </summary>
public class Projectile : MonoBehaviour
{
    private const int PlayerLayer = 3;
    private const float MaxLifetime = 5f;

    private int damage = 25;
    private ProjectilePool pool;
    private float spawnTime;

    /// <summary>Initialises damage, layer ignore, and optional pool reference.</summary>
    public void Initialize(int projectileDamage, ProjectilePool owningPool = null)
    {
        damage    = projectileDamage;
        pool      = owningPool;
        spawnTime = Time.time;

        Physics.IgnoreLayerCollision(gameObject.layer, PlayerLayer, true);
    }

    void Update()
    {
        // Auto-expire so pooled projectiles that miss are returned after MaxLifetime.
        if (Time.time - spawnTime >= MaxLifetime)
            Retire();
    }

    void OnCollisionEnter(Collision collision)
    {
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy == null)
            enemy = collision.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
            enemy.TakeDamage(damage);

        Retire();
    }

    private void Retire()
    {
        if (pool != null)
            pool.Release(this);
        else
            Destroy(gameObject);
    }
}
