using UnityEngine;

public class BasicEnemyFactory : MonoBehaviour, IGameObjectFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        return Instantiate(enemyPrefab, position, rotation);
    }
}