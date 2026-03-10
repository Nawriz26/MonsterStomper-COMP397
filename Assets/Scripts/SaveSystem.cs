using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved to: " + savePath);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            GameData data = JsonUtility.FromJson<GameData>(json);

            Debug.Log("Game Loaded");

            return data;
        }
        else
        {
            Debug.LogWarning("No Save File Found");

            return null;
        }
    }
}