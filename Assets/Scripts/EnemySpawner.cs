using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
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
        {
            nextSpawnTime = Time.time + spawnInterval;
        }
        else if (useWaves)
        {
            StartWave();
        }
    }

    void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (useWaves)
        {
            UpdateWaveSpawning();
        }
        else
        {
            UpdateContinuousSpawning();
        }
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
        Debug.Log($"Wave {currentWave} started!");
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
