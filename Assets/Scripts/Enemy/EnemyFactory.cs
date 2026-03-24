using UnityEngine;

public abstract class EnemyFactory : MonoBehaviour
{
    public abstract GameObject CreateEnemy(Vector3 position, Quaternion rotation);
}