// This file is intentionally empty.
// EnemyFactory is defined in /Assets/Scripts/EnemyFactory.cs.

using UnityEngine;

/// <summary>
/// Factory Method implementation for enemy creation.
/// Instantiates an enemy prefab and applies EnemyConfig stats to
/// EnemyHealth and EnemyController before the object activates.
/// </summary>
public class EnemyFactory : MonoBehaviour, IGameObjectFactory
{
    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Configuration")]
    [SerializeField] private EnemyConfig config;

    /// <summary>Creates a fully configured enemy at the given position and rotation.</summary>
    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyFactory: enemyPrefab is not assigned.");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, position, rotation);

        if (config != null)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.SetMaxHealth(config.maxHealth);

            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
                controller.Configure(
                    config.moveSpeed,
                    config.detectionRadius,
                    config.fieldOfView,
                    config.attackRange,
                    config.attackDamage,
                    config.attackInterval);
        }

        return enemy;
    }
}
// This stub was left by a Git merge conflict and has been cleared to avoid duplicate class errors.