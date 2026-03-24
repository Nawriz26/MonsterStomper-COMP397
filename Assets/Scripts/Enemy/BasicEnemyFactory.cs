using UnityEngine;

public class BasicEnemyFactory : EnemyFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public override GameObject CreateEnemy(Vector3 position, Quaternion rotation)
    {
        return Instantiate(enemyPrefab, position, rotation);
    }
}