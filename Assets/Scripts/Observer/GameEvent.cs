/// <summary>
/// Enumeration of all observable game events used by the Observer Pattern.
/// Both the Quest System and the Achievement System subscribe to these.
/// </summary>
public enum GameEvent
{
    // Player events
    PlayerMoved,
    PlayerJumped,
    PlayerAttacked,
    PlayerTookDamage,
    PlayerDied,

    // Collectible events
    CoinCollected,
    HealthPickedUp,

    // Combat events
    EnemyDefeated,

    // Game flow events
    GameStarted,
    GamePaused,
    GameResumed,
    GameOver,
    Victory,

    // Quest events
    QuestStarted,
    QuestStepCompleted,
    QuestCompleted,

    // Achievement events
    AchievementUnlocked
}
