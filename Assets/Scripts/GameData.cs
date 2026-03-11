using System;

/// <summary>
/// Human-readable serializable container for all saved game state.
/// </summary>
[Serializable]
public class GameData
{
    // --- Meta ---
    public int saveVersion = 1;
    public string saveDate;

    // --- Player ---
    public float playerX;
    public float playerY;
    public float playerZ;
    public int playerHealth;
    public int playerMaxHealth;

    // --- Game Stats ---
    public int currentScore;
    public int coins;
    public int enemiesDefeated;
    public float gameTimeSeconds;

    // --- Level ---
    public string levelName;
    public string lastCheckpoint;
}