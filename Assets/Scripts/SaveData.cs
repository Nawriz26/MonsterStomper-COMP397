using System;

/// <summary>
/// Serializable container for all game state that gets written to JSON.
/// </summary>
[Serializable]
public class SaveData
{
    public int saveVersion = 1;
    public string saveDate;

    public PlayerSaveData playerData;
    public GameSaveData gameData;
}

[Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public int currentHealth;
    public int maxHealth;
}

[Serializable]
public class GameSaveData
{
    public int currentScore;
    public int coinsCollected;
    public int enemiesDefeated;
    public float gameTimeSeconds;
    public string levelName;
}
