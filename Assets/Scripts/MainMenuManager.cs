using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    [Header("Load Game Panel")]
    [SerializeField] private GameObject loadGamePanel;

    void Start()
    {
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(false);
        }

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
        {
            GameManager.Instance.StartNewGame();
        }
        SceneManager.LoadScene("GamePlay");
    }

    public void LoadGame()
    {
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(true);
        }
        Debug.Log("Load Game pressed (not implemented yet)");
    }

    public void CloseLoadGamePanel()
    {
        if (loadGamePanel != null)
        {
            loadGamePanel.SetActive(false);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
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
}