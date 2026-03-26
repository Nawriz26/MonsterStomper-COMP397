using UnityEngine;

/// <summary>ScriptableObject holding all configurable stats for an enemy type.</summary>
[CreateAssetMenu(fileName = "EnemyConfig", menuName = "MonsterStomper/Factories/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Ground Patroller";

    [Header("Health")]
    public int maxHealth = 20;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float patrolRadius = 10f;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float fieldOfView = 120f;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackInterval = 1.5f;

    [Header("Rewards")]
    public int scoreReward = 100;
    public int coinDrop = 1;
}
