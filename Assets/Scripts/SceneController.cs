using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicForScene(currentScene);
        }

        UpdateGameState(currentScene);
    }

    private void UpdateGameState(string sceneName)
    {
        if (GameManager.Instance == null) return;

        switch (sceneName)
        {
            case "MainMenu":
                GameManager.Instance.SetGameState(GameState.MainMenu);
                break;
            case "GamePlay":
                GameManager.Instance.StartNewGame();
                GameManager.Instance.SetGameState(GameState.Playing);
                break;
            case "GameOver":
                break;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGamePlay()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("GamePlay");
    }

    public void LoadOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void LoadGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver");
    }

    public void RestartGame()
    {
        LoadGamePlay();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
