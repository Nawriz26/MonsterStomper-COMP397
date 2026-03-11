using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public Transform player;
    public PlayerHealth playerHealth;
    public int coins;

    public void SaveGame()
    {
        GameData data = new GameData();

        data.playerX = player.position.x;
        data.playerY = player.position.y;
        data.playerZ = player.position.z;

        data.playerHealth = playerHealth.GetCurrentHealth();

        data.coins = coins;

        SaveSystem.SaveGame(data);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();

        if (data != null)
        {
            Vector3 pos = new Vector3(data.playerX, data.playerY, data.playerZ);
            player.position = pos;

            playerHealth.SetHealth(data.playerHealth);

            coins = data.coins;

            Debug.Log("Game State Restored");
        }
    }
}