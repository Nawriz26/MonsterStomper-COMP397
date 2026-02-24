using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    [Header("Win Conditions")]
    [SerializeField] private WinType winType = WinType.DefeatAllEnemies;
    [SerializeField] private int requiredScore = 1000;
    [SerializeField] private int requiredCoins = 10;
    [SerializeField] private int requiredEnemies = 5;
    [SerializeField] private float timeLimit = 300f;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    private bool gameWon = false;

    void Update()
    {
        if (gameWon) return;

        if (GameManager.Instance == null) return;

        bool conditionMet = false;

        switch (winType)
        {
            case WinType.DefeatAllEnemies:
                if (enemySpawner != null)
                {
                    conditionMet = enemySpawner.GetActiveEnemyCount() == 0 && 
                                   GameManager.Instance.enemiesDefeated >= requiredEnemies;
                }
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
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        gameWon = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(true);
        }

        Invoke(nameof(LoadGameOverScene), 2f);
    }

    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}

public enum WinType
{
    DefeatAllEnemies,
    CollectCoins,
    ReachScore,
    SurviveTime
}
