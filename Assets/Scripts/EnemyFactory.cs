using UnityEngine;

/// <summary>
/// Factory Method implementation for enemy creation.
/// Instantiates an enemy prefab and applies EnemyConfig stats.
/// Assign a different EnemyConfig asset per enemy type (Patroller, Brute, Flying).
/// </summary>
public class EnemyFactory : MonoBehaviour, IGameObjectFactory
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Configuration")]
    [SerializeField] private EnemyConfig config;

    /// <summary>Creates a configured enemy at the given position and rotation.</summary>
    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyFactory: enemyPrefab is not assigned.");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, position, rotation);

        if (config == null)
        {
            Debug.LogWarning("EnemyFactory: no EnemyConfig assigned — using prefab defaults.");
            return enemy;
        }

        ApplyConfig(enemy);
        return enemy;
    }

    private void ApplyConfig(GameObject enemy)
    {
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
            health.SetMaxHealth(config.maxHealth);

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
            controller.Configure(config.moveSpeed, config.detectionRadius,
                                 config.fieldOfView, config.attackRange,
                                 config.attackDamage, config.attackInterval);
    }
}
