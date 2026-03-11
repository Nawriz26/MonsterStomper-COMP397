using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState = GameState.MainMenu;

    [Header("Game Stats")]
    public int currentScore = 0;
    public int enemiesDefeated = 0;
    public int coinsCollected = 0;
    public float gameTime = 0f;

    [Header("Settings")]
    public bool isPaused = false;

    public delegate void OnGameStateChanged(GameState newState);
    public event OnGameStateChanged GameStateChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: Instance created");
        }
        else
        {
            Debug.Log("GameManager: Duplicate instance destroyed");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing && !isPaused)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void SetGameState(GameState newState)
    {
        Debug.Log($"GameManager: State changed from {currentState} to {newState}");
        currentState = newState;
        GameStateChanged?.Invoke(newState);
    }

    public void AddScore(int points)
    {
        currentScore += points;
    }

    public void AddCoin()
    {
        coinsCollected++;
        AddScore(10);
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        AddScore(100);
    }

    /// <summary>
    /// When true, SaveManager will auto-restore the last save as soon as GamePlay loads.
    /// Set this before loading the GamePlay scene from the Main Menu.
    /// </summary>
    public bool PendingLoad { get; private set; } = false;

    /// <summary>Signals that the next GamePlay session should restore from save.</summary>
    public void RequestLoadGame()
    {
        PendingLoad = true;
        SetGameState(GameState.Playing);
        Debug.Log("GameManager: PendingLoad set — save will be restored on GamePlay load.");
    }

    /// <summary>Clears the pending load flag after SaveManager has consumed it.</summary>
    public void ClearPendingLoad()
    {
        PendingLoad = false;
    }

    public void StartNewGame()
    {
        Debug.Log("GameManager: StartNewGame() called - Resetting stats");
        currentScore = 0;
        enemiesDefeated = 0;
        coinsCollected = 0;
        gameTime = 0f;
        isPaused = false;
        PendingLoad = false;
        SetGameState(GameState.Playing);
    }

    public void GameOver(bool won)
    {
        SetGameState(won ? GameState.Victory : GameState.GameOver);
        isPaused = false;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory
}
