using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 25;

    public void Initialize(int projectileDamage)
    {
        damage = projectileDamage;
    }

    void OnCollisionEnter(Collision collision)
    {
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
