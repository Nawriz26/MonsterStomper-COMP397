using UnityEngine;

/// <summary>
/// Projectile fired by the player.
/// Damages enemies on contact and destroys itself on any collision.
/// Ignores the Player layer on spawn so it never bounces back off the shooter.
/// </summary>
public class Projectile : MonoBehaviour
{
    private const int PlayerLayer = 3;  // "Player" layer index

    private int damage = 25;

    /// <summary>Sets the damage value and ignores collision with the Player layer.</summary>
    public void Initialize(int projectileDamage)
    {
        damage = projectileDamage;

        // Ignore physics collision with every collider on the Player layer
        // so the bullet never hits the character that fired it
        Physics.IgnoreLayerCollision(gameObject.layer, PlayerLayer, true);
    }

    void OnCollisionEnter(Collision collision)
    {
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy == null)
            enemy = collision.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
