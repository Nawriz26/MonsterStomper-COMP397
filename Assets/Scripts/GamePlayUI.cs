using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePlayUI : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Health Bar")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("MiniMap")]
    [SerializeField] private RawImage miniMapDisplay;

    [Header("Inventory Panel")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI inventoryCountText;

    private PlayerHealth playerHealth;

    void Awake()
    {
        // Subscribe in Awake so the listener is registered before
        // PlayerHealth.Start() fires OnHealthChanged with the initial value.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // Sync the bar to the current health value in case Awake ran
        // before PlayerHealth initialised currentHealth.
        if (playerHealth != null)
            UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = $"Score: {GameManager.Instance.currentScore}";
        if (coinsText != null)
            coinsText.text = $"Coins: {GameManager.Instance.coinsCollected}";
        if (enemiesText != null)
            enemiesText.text = $"Enemies: {GameManager.Instance.enemiesDefeated}";
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(GameManager.Instance.gameTime / 60f);
            int seconds = Mathf.FloorToInt(GameManager.Instance.gameTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    /// <summary>Called automatically by PlayerHealth.OnHealthChanged event.</summary>
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
            healthBar.value = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        if (healthText != null)
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }

    public void UpdateInventoryCount(int count)
    {
        if (inventoryCountText != null)
            inventoryCountText.text = $"Items: {count}/{GameConstants.MAX_INVENTORY_SLOTS}";
    }
}
