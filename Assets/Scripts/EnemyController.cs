using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointReachDistance = 1f;

    private NavMeshAgent agent;
    private Transform player;
    private EnemyHealth enemyHealth;
    private EnemyState currentState = EnemyState.Patrol;

    private int currentPatrolIndex = 0;
    private float nextAttackTime = 0f;
    private bool playerDetected = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (patrolPoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            GoToNextPatrolPoint();
        }
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
            return;

        DetectPlayer();

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle <= fieldOfView / 2f)
            {
                if (HasLineOfSight(player.position))
                {
                    playerDetected = true;

                    if (distanceToPlayer <= attackRange)
                    {
                        currentState = EnemyState.Attack;
                    }
                    else
                    {
                        currentState = EnemyState.Chase;
                    }
                    return;
                }
            }
        }

        if (playerDetected && distanceToPlayer > detectionRadius * 1.5f)
        {
            playerDetected = false;
            currentState = EnemyState.Patrol;
        }
    }

    private bool HasLineOfSight(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        Ray ray = new Ray(transform.position + Vector3.up, directionToTarget.normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToTarget, obstacleLayers))
        {
            return false;
        }

        return true;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < waypointReachDistance)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void Chase()
    {
        if (player == null) return;

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    private void Attack()
    {
        if (player == null) return;

        agent.SetDestination(transform.position);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void PerformAttack()
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHit();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null && playerDetected)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}

public enum EnemyState
{
    Patrol,
    Chase,
    Attack
}
