using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the in-game Game Over overlay panel.
/// This component must be placed on an ALWAYS-ACTIVE parent so its Start()
/// runs at scene load. It then holds a reference to the GameOverPanel
/// and activates it when the player dies.
/// </summary>
public class GameOverPanelController : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("The GameOverPanel GameObject to show on death. Must be a separate, initially-inactive child.")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Player Reference")]
    [Tooltip("Assign the Player GameObject that has a PlayerHealth component.")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private const string GamePlayScene = "GamePlay";
    private const string MainMenuScene = "MainMenu";

    void Start()
    {
        // Fall back to scene search if not assigned in Inspector.
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.OnDeath.AddListener(Show);
        else
            Debug.LogWarning("GameOverPanelController: No PlayerHealth found — Game Over will not trigger.");

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        // Ensure panel is hidden at start.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath.RemoveListener(Show);
    }

    /// <summary>Reveals the Game Over overlay and freezes time.</summary>
    public void Show()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            GameManager.Instance.StartNewGame();

        SceneManager.LoadScene(GamePlayScene);
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }
}
