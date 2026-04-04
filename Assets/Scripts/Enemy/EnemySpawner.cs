using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns enemies using EnemyFactory (Factory Method pattern).
/// Supports continuous and wave-based modes.
/// Assign an EnemyFactory component to the factory field; the legacy prefab field
/// remains as a fallback so existing scenes keep working without reassignment.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Factory (Factory Method Pattern)")]
    [Tooltip("Assign a GameObject that has an EnemyFactory component.")]
    [SerializeField] private EnemyFactory enemyFactory;

    [Header("Fallback — Legacy Direct Spawn (used only if no factory assigned)")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Wave Settings")]
    [SerializeField] private bool useWaves = false;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float timeBetweenWaves = 10f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float nextSpawnTime = 0f;
    private int currentWave = 0;
    private int enemiesSpawnedThisWave = 0;

    void Start()
    {
        if (spawnOnStart && !useWaves)
            nextSpawnTime = Time.time + spawnInterval;
        else if (useWaves)
            StartWave();
    }

    void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (useWaves)
            UpdateWaveSpawning();
        else
            UpdateContinuousSpawning();
    }

    private void UpdateContinuousSpawning()
    {
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void UpdateWaveSpawning()
    {
        if (enemiesSpawnedThisWave < enemiesPerWave)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnEnemy();
                enemiesSpawnedThisWave++;
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
        else if (activeEnemies.Count == 0)
        {
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    private void StartWave()
    {
        currentWave++;
        enemiesSpawnedThisWave = 0;
        nextSpawnTime = Time.time;
        Debug.Log($"EnemySpawner: Wave {currentWave} started.");
    }

    /// <summary>Spawns one enemy via the factory, or falls back to direct Instantiate.</summary>
    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: no spawn points assigned.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = null;

        if (enemyFactory != null)
        {
            enemy = enemyFactory.Create(spawnPoint.position, spawnPoint.rotation);
        }
        else if (enemyPrefab != null)
        {
            enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogError("EnemySpawner: assign either an EnemyFactory or a fallback enemyPrefab.");
            return;
        }

        if (enemy != null)
            activeEnemies.Add(enemy);
    }

    public int GetActiveEnemyCount() => activeEnemies.Count;
    public int GetCurrentWave() => currentWave;
}
