using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private int healAmount = 25;
    [SerializeField] private bool rotateObject = true;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;

    /// <summary>Applies a CollectibleConfig from the CollectibleFactory.</summary>
    public void Configure(CollectibleConfig config)
    {
        type = config.type;
        scoreValue = config.scoreValue;
        healAmount = config.healAmount;
    }

    void Update()
    {
        if (rotateObject)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }
    }

    private void CollectItem(GameObject player)
    {
        if (GameManager.Instance != null)
        {
            switch (type)
            {
                case CollectibleType.Coin:
                    GameManager.Instance.AddCoin();
                    GameEventBus.Raise(GameEvent.CoinCollected);
                    break;
                case CollectibleType.Health:
                    PlayerHealth health = player.GetComponent<PlayerHealth>();
                    if (health != null)
                        health.Heal(healAmount);
                    GameManager.Instance.AddScore(scoreValue);
                    GameEventBus.Raise(GameEvent.HealthPickedUp);
                    break;
                case CollectibleType.Points:
                    GameManager.Instance.AddScore(scoreValue);
                    GameEventBus.Raise(GameEvent.CoinCollected);
                    break;
            }
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoinCollect();

        HapticManager.Instance?.OnCollect();

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

public enum CollectibleType
{
    Coin,
    Health,
    Points
}
