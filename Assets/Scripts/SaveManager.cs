using UnityEngine;
using TMPro;

/// <summary>
/// Scene-level component that bridges PauseMenuController with SaveSystem.
/// Auto-finds the Player if references are not assigned.
/// </summary>
public class SaveManager : MonoBehaviour
{
    [Header("References (auto-found if empty)")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform playerTransform;

    [Header("UI Feedback")]
    [SerializeField] private TextMeshProUGUI saveConfirmationText;

    private const float ConfirmationDisplayDuration = 2f;
    private float confirmationTimer = 0f;

    void Start()
    {
        FindPlayerReferences();

        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (confirmationTimer > 0f)
        {
            confirmationTimer -= Time.unscaledDeltaTime;
            if (confirmationTimer <= 0f && saveConfirmationText != null)
            {
                saveConfirmationText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>Saves current game state and shows confirmation message.</summary>
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

        Debug.Log(success ? $"SaveManager: Saved to {SaveSystem.GetSavePath()}" : "SaveManager: Save failed.");
    }

    /// <summary>Loads saved data back into the scene. Position and health are restored.</summary>
    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();

        if (data == null)
        {
            Debug.LogWarning("SaveManager: No save file found. Nothing to load.");
            return;
        }

        FindPlayerReferences();

        // Restore player position
        if (playerTransform != null)
        {
            playerTransform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        }

        // Restore player health
        if (playerHealth != null)
        {
            playerHealth.SetHealth(data.playerHealth);
        }

        // Restore game stats
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentScore = data.currentScore;
            GameManager.Instance.coinsCollected = data.coins;
            GameManager.Instance.enemiesDefeated = data.enemiesDefeated;
            GameManager.Instance.gameTime = data.gameTimeSeconds;
        }

        Debug.Log($"SaveManager: Game state restored from save dated {data.saveDate}.");
    }

    private void FindPlayerReferences()
    {
        if (playerHealth == null || playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (playerHealth == null)
                    playerHealth = player.GetComponent<PlayerHealth>();
                if (playerTransform == null)
                    playerTransform = player.transform;
            }
        }
    }
}