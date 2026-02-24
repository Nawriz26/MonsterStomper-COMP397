using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;
    [SerializeField] private TextMeshProUGUI enemiesDefeatedText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    void Start()
    {
        DisplayGameStats();
        AddButtonListeners();
    }

    private void DisplayGameStats()
    {
        if (GameManager.Instance == null) return;

        if (titleText != null)
        {
            bool won = GameManager.Instance.currentState == GameState.Victory;
            titleText.text = won ? "VICTORY!" : "GAME OVER";
            titleText.color = won ? Color.green : Color.red;
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.Instance.currentScore}";
        }

        if (coinsCollectedText != null)
        {
            coinsCollectedText.text = $"Coins Collected: {GameManager.Instance.coinsCollected}";
        }

        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = $"Enemies Defeated: {GameManager.Instance.enemiesDefeated}";
        }

        if (timePlayedText != null)
        {
            int minutes = Mathf.FloorToInt(GameManager.Instance.gameTime / 60f);
            int seconds = Mathf.FloorToInt(GameManager.Instance.gameTime % 60f);
            timePlayedText.text = $"Time Played: {minutes:00}:{seconds:00}";
        }
    }

    private void AddButtonListeners()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    public void PlayAgain()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }

        SceneManager.LoadScene("GamePlay");
    }

    public void ReturnToMainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
