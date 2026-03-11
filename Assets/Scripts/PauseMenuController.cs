using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Save/Load Panels")]
    [SerializeField] private GameObject savePanel;
    [SerializeField] private GameObject loadPanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private bool isPaused = false;
    private SaveManager saveManager;

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (savePanel != null) savePanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);

        saveManager = FindObjectOfType<SaveManager>(true);

        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        if (resumeButton != null)   resumeButton.onClick.AddListener(ResumeGame);
        if (saveButton != null)     saveButton.onClick.AddListener(SaveGame);
        if (loadButton != null)     loadButton.onClick.AddListener(OpenLoadPanel);
        if (optionsButton != null)  optionsButton.onClick.AddListener(OpenOptions);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.PauseGame();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (savePanel != null) savePanel.SetActive(false);
        if (loadPanel != null) loadPanel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
    }

    /// <summary>Triggers a full JSON save via SaveManager.</summary>
    public void SaveGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();

        if (saveManager == null)
            saveManager = FindObjectOfType<SaveManager>(true);

        if (saveManager != null)
            saveManager.SaveGame();
        else
            Debug.LogWarning("PauseMenuController: SaveManager not found in scene.");
    }

    public void OpenLoadPanel()
    {
        if (loadPanel != null) loadPanel.SetActive(true);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
    }

    public void CloseLoadPanel()
    {
        if (loadPanel != null) loadPanel.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
    }

    public void OpenOptions()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("Options");
    }

    public void ReturnToMainMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonClick();
        ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}
