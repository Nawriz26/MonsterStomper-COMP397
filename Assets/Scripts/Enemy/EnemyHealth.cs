using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHit();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyDeath();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyDefeated();

            int kills = GameManager.Instance.enemiesDefeated;

            if (AchievementManager.Instance != null)
            {
                if (kills == 1)
                {
                    AchievementManager.Instance.Unlock("FIRST_KILL");
                }

                if (kills >= 10)
                {
                    AchievementManager.Instance.Unlock("MONSTER_SLAYER");
                }
            }
        }

        GameEventBus.Raise(GameEvent.EnemyDefeated);

        // Return to pool if one exists, otherwise fall back to Destroy.
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.Release(this);
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }

    /// <summary>Resets health to max. Called when an enemy is recycled from the pool.</summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>Called by EnemyFactory to override the default max health before Start() fires.</summary>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
    }
}