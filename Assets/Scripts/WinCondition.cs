using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    [Header("Win Conditions")]
    [SerializeField] private WinType winType = WinType.DefeatAllEnemies;
    [SerializeField] private int requiredScore = 1000;
    [SerializeField] private int requiredCoins = 10;
    [SerializeField] private int requiredEnemies = 10;
    [SerializeField] private float timeLimit = 300f;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Win Panel")]
    [Tooltip("The You Win panel GameObject — initially inactive.")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    private const string GamePlayScene = "GamePlay";
    private const string MainMenuScene = "MainMenu";

    private bool gameWon = false;

    void Start()
    {
        if (enemySpawner == null)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (gameWon) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentState != GameState.Playing) return;

        bool conditionMet = false;

        switch (winType)
        {
            case WinType.DefeatAllEnemies:
                conditionMet = GameManager.Instance.enemiesDefeated >= requiredEnemies;
                break;

            case WinType.CollectCoins:
                conditionMet = GameManager.Instance.coinsCollected >= requiredCoins;
                break;

            case WinType.ReachScore:
                conditionMet = GameManager.Instance.currentScore >= requiredScore;
                break;

            case WinType.SurviveTime:
                conditionMet = GameManager.Instance.gameTime >= timeLimit;
                break;
        }

        if (conditionMet)
            WinGame();
    }

    /// <summary>Triggers the win state: shows the panel and freezes time.</summary>
    private void WinGame()
    {
        gameWon = true;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver(true);

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void OnPlayAgainClicked()
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

public enum WinType
{
    DefeatAllEnemies,
    CollectCoins,
    ReachScore,
    SurviveTime
}
