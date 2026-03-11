using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    [Header("Load Game Panel")]
    [SerializeField] private GameObject loadGamePanel;
    [SerializeField] private TextMeshProUGUI saveInfoText;
    [SerializeField] private Button confirmLoadButton;

    void Start()
    {
        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);

        // Disable Load button if no save exists
        if (loadGameButton != null)
            loadGameButton.interactable = SaveSystem.SaveExists();

        AddButtonSounds();
    }

    private void AddButtonSounds()
    {
        if (AudioManager.Instance == null) return;

        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (optionsButton != null)
            optionsButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        if (exitButton != null)
            exitButton.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartNewGame();

        SceneManager.LoadScene("GamePlay");
    }

    /// <summary>Opens the load panel and populates it with save metadata.</summary>
    public void LoadGame()
    {
        if (!SaveSystem.SaveExists())
        {
            if (saveInfoText != null)
                saveInfoText.text = "No save file found.";
            if (confirmLoadButton != null)
                confirmLoadButton.interactable = false;
            if (loadGamePanel != null)
                loadGamePanel.SetActive(true);
            return;
        }

        GameData preview = SaveSystem.LoadGame();
        if (saveInfoText != null && preview != null)
        {
            saveInfoText.text =
                $"<b>Save Found</b>\n" +
                $"Date: {preview.saveDate}\n" +
                $"Level: {preview.levelName}\n" +
                $"Score: {preview.currentScore}  |  Coins: {preview.coins}\n" +
                $"HP: {preview.playerHealth}/{preview.playerMaxHealth}  |  Enemies: {preview.enemiesDefeated}\n" +
                $"Time: {FormatTime(preview.gameTimeSeconds)}";
        }

        if (confirmLoadButton != null)
            confirmLoadButton.interactable = true;

        if (loadGamePanel != null)
            loadGamePanel.SetActive(true);
    }

    /// <summary>Loads the GamePlay scene with a pending restore flag set on GameManager.</summary>
    public void ConfirmLoad()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (GameManager.Instance != null)
            GameManager.Instance.RequestLoadGame();

        SceneManager.LoadScene("GamePlay");
    }

    public void CloseLoadGamePanel()
    {
        if (loadGamePanel != null)
            loadGamePanel.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        Debug.Log("Exit Game clicked (only works in builds, not in editor)");
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}