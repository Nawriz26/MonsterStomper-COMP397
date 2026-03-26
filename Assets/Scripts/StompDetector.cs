using UnityEngine;

/// <summary>
/// Detects when the player lands on top of an enemy and instantly kills it.
/// Tracks fall velocity independently because CharacterController.velocity.y
/// is clamped to 0 the moment the controller resolves ground contact,
/// making it unreliable for stomp detection.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class StompDetector : MonoBehaviour
{
    [Header("Stomp Settings")]
    [Tooltip("Upward velocity applied to the player after a successful stomp.")]
    [SerializeField] private float stompBounceForce = 6f;

    [Tooltip("Minimum downward speed required to register a stomp. Lower = easier to trigger.")]
    [SerializeField] private float minFallSpeed = 1f;

    [Tooltip("Contact normal Y threshold to count as landing on top. 0.4 = within ~66° of straight up.")]
    [SerializeField] private float topContactThreshold = 0.4f;

    private PlayerController playerController;
    private float trackedVerticalVelocity;
    private Vector3 previousPosition;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        previousPosition = transform.position;
    }

    void Update()
    {
        // Derive vertical velocity from position delta — reliable regardless of
        // CharacterController internals which clamp velocity.y to 0 on ground contact
        if (Time.deltaTime > 0f)
            trackedVerticalVelocity = (transform.position.y - previousPosition.y) / Time.deltaTime;

        previousPosition = transform.position;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Must be moving downward fast enough
        if (trackedVerticalVelocity > -minFallSpeed) return;

        // Contact must be on the top surface of the enemy collider
        if (hit.normal.y < topContactThreshold) return;

        // Walk up the hierarchy to find EnemyHealth
        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
            enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null) return;
        if (enemyHealth.GetCurrentHealth() <= 0) return;

        // Instant kill — exceeds max HP so TakeDamage always triggers Die()
        enemyHealth.TakeDamage(enemyHealth.GetMaxHealth() + 1);

        // Bounce player upward
        playerController.ApplyStompBounce(stompBounceForce);

        HapticManager.Instance?.OnEnemyKilled();
    }
}
