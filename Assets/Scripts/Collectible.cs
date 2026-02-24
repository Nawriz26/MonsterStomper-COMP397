using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool rotateObject = true;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;

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
                    break;
                case CollectibleType.Health:
                    PlayerHealth health = player.GetComponent<PlayerHealth>();
                    if (health != null)
                    {
                        health.Heal(25);
                    }
                    GameManager.Instance.AddScore(scoreValue);
                    break;
                case CollectibleType.Points:
                    GameManager.Instance.AddScore(scoreValue);
                    break;
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinCollect();
        }

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
