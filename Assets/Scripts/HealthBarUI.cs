using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorGradient;

    [Header("Target")]
    [SerializeField] private PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthSlider.value = healthPercentage;

            if (fillImage != null && colorGradient != null)
            {
                fillImage.color = colorGradient.Evaluate(healthPercentage);
            }
        }
    }
}
