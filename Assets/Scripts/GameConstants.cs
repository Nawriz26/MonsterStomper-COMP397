using UnityEngine;

public static class GameConstants
{
    public const float PLAYER_MOVE_SPEED = 5f;
    public const float PLAYER_SPRINT_SPEED = 8f;
    public const float PLAYER_ROTATION_SPEED = 10f;
    public const float PLAYER_JUMP_FORCE = 5f;
    
    public const int PLAYER_MAX_HEALTH = 100;
    public const int ENEMY_MAX_HEALTH = 50;
    
    public const float ENEMY_DETECTION_RADIUS = 15f;
    public const float ENEMY_ATTACK_RANGE = 2f;
    public const float ENEMY_MOVE_SPEED = 3f;
    public const float ENEMY_ATTACK_COOLDOWN = 2f;
    public const int ENEMY_DAMAGE = 10;
    
    public const int COIN_SCORE_VALUE = 10;
    public const int ENEMY_KILL_SCORE = 100;
    
    public const int MAX_INVENTORY_SLOTS = 10;
    
    public const string LAYER_PLAYER = "Player";
    public const string LAYER_ENEMY = "Default";
    public const string LAYER_GROUND = "Default";
    
    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_COIN = "Coin";
}
