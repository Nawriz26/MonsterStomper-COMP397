using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Serializes game state to a human-readable JSON file.
/// On WebGL, falls back to PlayerPrefs since System.IO is not available.
/// </summary>
public static class SaveSystem
{
    private const string SaveFileName = "save.json";
    private const string PlayerPrefsKey = "MonsterStomperSaveData";

    /// <summary>Returns the platform-appropriate save path (Editor/Desktop only).</summary>
    public static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    /// <summary>
    /// Builds a GameData snapshot and writes it to JSON.
    /// Returns true on success.
    /// </summary>
    public static bool SaveGame(PlayerHealth playerHealth, Transform playerTransform)
    {
        try
        {
            GameData data = BuildGameData(playerHealth, playerTransform);
            string json = JsonUtility.ToJson(data, prettyPrint: true);

#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
            Debug.Log($"SaveSystem: Saved to PlayerPrefs (WebGL).\n{json}");
#else
            string path = GetSavePath();
            File.WriteAllText(path, json);
            Debug.Log($"SaveSystem: Saved to {path}\n{json}");
#endif
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveSystem: Save failed — {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Reads the save file. Returns null if no save exists or on error.
    /// </summary>
    public static GameData LoadGame()
    {
        try
        {
            string json = string.Empty;

#if UNITY_WEBGL && !UNITY_EDITOR
            if (!PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                Debug.Log("SaveSystem: No save data in PlayerPrefs.");
                return null;
            }
            json = PlayerPrefs.GetString(PlayerPrefsKey);
#else
            string path = GetSavePath();
            if (!File.Exists(path))
            {
                Debug.Log($"SaveSystem: No save file at {path}.");
                return null;
            }
            json = File.ReadAllText(path);
#endif
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"SaveSystem: Loaded save from {data.saveDate}.");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveSystem: Load failed — {e.Message}");
            return null;
        }
    }

    /// <summary>Returns true if a save exists.</summary>
    public static bool SaveExists()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return PlayerPrefs.HasKey(PlayerPrefsKey);
#else
        return File.Exists(GetSavePath());
#endif
    }

    /// <summary>Deletes the existing save.</summary>
    public static void DeleteSave()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.Save();
#else
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("SaveSystem: Save file deleted.");
        }
#endif
    }

    private static GameData BuildGameData(PlayerHealth playerHealth, Transform playerTransform)
    {
        Vector3 pos = playerTransform != null ? playerTransform.position : Vector3.zero;

        return new GameData
        {
            saveVersion = 1,
            saveDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),

            playerX = pos.x,
            playerY = pos.y,
            playerZ = pos.z,
            playerHealth = playerHealth != null ? playerHealth.GetCurrentHealth() : 100,
            playerMaxHealth = playerHealth != null ? playerHealth.GetMaxHealth() : 100,

            currentScore = GameManager.Instance != null ? GameManager.Instance.currentScore : 0,
            coins = GameManager.Instance != null ? GameManager.Instance.coinsCollected : 0,
            enemiesDefeated = GameManager.Instance != null ? GameManager.Instance.enemiesDefeated : 0,
            gameTimeSeconds = GameManager.Instance != null ? GameManager.Instance.gameTime : 0f,
            levelName = SceneManager.GetActiveScene().name,
            lastCheckpoint = string.Empty
        };
    }
}