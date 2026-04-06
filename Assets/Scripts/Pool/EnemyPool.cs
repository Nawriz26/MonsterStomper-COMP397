using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Singleton pool for enemy GameObjects.
/// EnemySpawner uses this instead of Instantiate to recycle enemies
/// and avoid per-spawn allocations on mobile.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 15;

    private ObjectPool<EnemyHealth> pool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        EnemyHealth prefabHealth = enemyPrefab.GetComponent<EnemyHealth>();
        pool = new ObjectPool<EnemyHealth>(prefabHealth, initialPoolSize, transform);
    }

    /// <summary>Spawns an enemy at the given position from the pool.</summary>
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        EnemyHealth enemy = pool.Get();
        enemy.transform.SetPositionAndRotation(position, rotation);

        // Re-enable NavMeshAgent so it registers on the NavMesh at the new position
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
            agent.enabled = true;
        }

        // Reset health so recycled enemies start at full health
        enemy.ResetHealth();

        return enemy.gameObject;
    }

    /// <summary>Returns an enemy to the pool instead of destroying it.</summary>
    public void Release(EnemyHealth enemy)
    {
        pool.Release(enemy);
    }
}
