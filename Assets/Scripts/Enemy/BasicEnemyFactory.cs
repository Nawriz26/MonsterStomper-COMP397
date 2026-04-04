<<<<<<< HEAD
// This file is intentionally empty.
// BasicEnemyFactory was part of an older branch pattern using an abstract EnemyFactory base.
// The project now uses EnemyFactory : IGameObjectFactory (see /Assets/Scripts/EnemyFactory.cs).
=======
using UnityEngine;

public class BasicEnemyFactory : MonoBehaviour, IGameObjectFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        return Instantiate(enemyPrefab, position, rotation);
    }
}
>>>>>>> 254a1c8edb0db4e9de617a27d88da2b1b7d2896c
