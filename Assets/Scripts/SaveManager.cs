using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Scene-level component that bridges PauseMenuController with SaveSystem.
/// Auto-finds the Player if references are not assigned.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    [Header("References (auto-found if empty)")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform playerTransform;

    [Header("UI Feedback")]
    [SerializeField] private TextMeshProUGUI saveConfirmationText;
    [SerializeField] private TextMeshProUGUI loadStatusText;

    [Header("Load Panel")]
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private TextMeshProUGUI saveInfoText;

    /// <summary>Fired after a successful load so PauseMenuController can close the panel.</summary>
    public UnityEvent OnLoadSuccess = new UnityEvent();

    private const float ConfirmationDisplayDuration = 2f;
    private float confirmationTimer = 0f;
    private float loadStatusTimer = 0f;

    void Start()
    {
        FindPlayerReferences();

        if (saveConfirmationText != null)
            saveConfirmationText.gameObject.SetActive(false);

        if (loadStatusText != null)
            loadStatusText.gameObject.SetActive(false);

        // Auto-restore if GameManager requested a load from the Main Menu
        if (GameManager.Instance != null && GameManager.Instance.PendingLoad)
        {
            GameManager.Instance.ClearPendingLoad();
            ConfirmLoad();
            Debug.Log("SaveManager: Auto-restored save state from Main Menu.");
        }
    }

    void Update()
    {
        if (confirmationTimer > 0f)
        {
            confirmationTimer -= Time.unscaledDeltaTime;
            if (confirmationTimer <= 0f && saveConfirmationText != null)
                saveConfirmationText.gameObject.SetActive(false);
        }

        if (loadStatusTimer > 0f)
        {
            loadStatusTimer -= Time.unscaledDeltaTime;
            if (loadStatusTimer <= 0f && loadStatusText != null)
                loadStatusText.gameObject.SetActive(false);
        }
    }

    /// <summary>Saves current game state and shows a confirmation message.</summary>
    public void SaveGame()
    {
        FindPlayerReferences();

        bool success = SaveSystem.SaveGame(playerHealth, playerTransform);

        if (saveConfirmationText != null)
        {
            saveConfirmationText.text = success ? "Game Saved!" : "Save Failed!";
            saveConfirmationText.gameObject.SetActive(true);
            confirmationTimer = ConfirmationDisplayDuration;
        }

        Debug.Log(success
            ? $"SaveManager: Saved to {SaveSystem.GetSavePath()}"
            : "SaveManager: Save failed.");
    }

    /// <summary>
    /// Opens the load panel and populates it with metadata from the existing save file.
    /// Call this when the player clicks the Load button.
    /// </summary>
    public void OpenLoadPanel()
    {
        if (loadPanel == null) return;

        if (SaveSystem.SaveExists())
        {
            GameData preview = SaveSystem.LoadGame();
            if (preview != null && saveInfoText != null)
            {
                saveInfoText.text =
                    $"<b>Save Found</b>\n" +
                    $"Date: {preview.saveDate}\n" +
                    $"Level: {preview.levelName}\n" +
                    $"Score: {preview.currentScore}  |  Coins: {preview.coins}\n" +
                    $"HP: {preview.playerHealth}/{preview.playerMaxHealth}  |  Enemies: {preview.enemiesDefeated}\n" +
                    $"Time: {FormatTime(preview.gameTimeSeconds)}";
            }
        }
        else
        {
            if (saveInfoText != null)
                saveInfoText.text = "No save file found.";
        }

        loadPanel.SetActive(true);
    }

    /// <summary>Confirms the load — restores all game state and resumes play.</summary>
    public void ConfirmLoad()
    {
        GameData data = SaveSystem.LoadGame();

        if (data == null)
        {
            ShowLoadStatus("No save file found!");
            return;
        }

        FindPlayerReferences();

        // Restore player position
        if (playerTransform != null)
            playerTransform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

        // Restore player health
        if (playerHealth != null)
            playerHealth.SetHealth(data.playerHealth);

        // Restore game stats
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentScore = data.currentScore;
            GameManager.Instance.coinsCollected = data.coins;
            GameManager.Instance.enemiesDefeated = data.enemiesDefeated;
            GameManager.Instance.gameTime = data.gameTimeSeconds;
        }

        Debug.Log($"SaveManager: Game state restored from save dated {data.saveDate}.");

        OnLoadSuccess?.Invoke();
    }

    /// <summary>Closes the load panel without loading.</summary>
    public void CloseLoadPanel()
    {
        if (loadPanel != null)
            loadPanel.SetActive(false);
    }

    private void ShowLoadStatus(string message)
    {
        if (loadStatusText == null) return;
        loadStatusText.text = message;
        loadStatusText.gameObject.SetActive(true);
        loadStatusTimer = ConfirmationDisplayDuration;
    }

    private void FindPlayerReferences()
    {
        if (playerHealth != null && playerTransform != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerHealth == null)
                playerHealth = player.GetComponent<PlayerHealth>();
            if (playerTransform == null)
                playerTransform = player.transform;
        }
    }

    private static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}