using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Observer that tracks and unlocks achievements.
/// Subscribes to the GameEventBus and evaluates each AchievementData's unlock condition.
/// </summary>
public class AchievementManager : MonoBehaviour, IGameObserver
{
    public static AchievementManager Instance { get; private set; }

    [Header("Achievements")]
    [SerializeField] private List<AchievementData> achievements = new List<AchievementData>();

    [Header("UI")]
    [SerializeField] private AchievementNotificationUI notificationUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadProgress();
    }

    void Start()
    {
        // Register in Start (not OnEnable) so GameEventBus.Instance is guaranteed
        // to be set after all Awake calls have completed.
        if (GameEventBus.Instance != null)
            GameEventBus.Instance.RegisterObserver(this);
        else
            Debug.LogWarning("AchievementManager: GameEventBus instance not found during Start.");
    }

    void OnDestroy()
    {
        if (GameEventBus.Instance != null)
            GameEventBus.Instance.UnregisterObserver(this);
    }

    // ── IGameObserver ─────────────────────────────────────────────────────────

    /// <summary>Receives all game events and checks whether any achievement progresses.</summary>
    public void OnNotify(GameEvent gameEvent, object data)
    {
        foreach (AchievementData achievement in achievements)
        {
            if (achievement.isUnlocked) continue;
            if (achievement.triggerEvent != gameEvent) continue;

            achievement.currentCount++;

            if (achievement.currentCount >= achievement.requiredCount)
                UnlockAchievement(achievement);
        }
    }

    // ── Achievement Logic ─────────────────────────────────────────────────────

    private void UnlockAchievement(AchievementData achievement)
    {
        achievement.isUnlocked = true;
        SaveProgress();

        Debug.Log($"AchievementManager: Unlocked — {achievement.title}");

        if (notificationUI != null)
            notificationUI.ShowNotification(achievement);

        // Broadcast the unlock so other systems can react (e.g., QuestManager)
        GameEventBus.Raise(GameEvent.AchievementUnlocked, achievement);
    }

    /// <summary>Returns all achievements (used by the achievement list UI).</summary>
    public List<AchievementData> GetAllAchievements() => achievements;

    /// <summary>Returns true if the achievement with the given id has been unlocked.</summary>
    public bool IsUnlocked(string achievementId)
    {
        AchievementData a = achievements.Find(x => x.achievementId == achievementId);
        return a != null && a.isUnlocked;
    }

    // ── Persistence ───────────────────────────────────────────────────────────

    private void SaveProgress()
    {
        foreach (AchievementData a in achievements)
        {
            PlayerPrefs.SetInt($"Achievement_{a.achievementId}_Unlocked", a.isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt($"Achievement_{a.achievementId}_Count",    a.currentCount);
        }
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        foreach (AchievementData a in achievements)
        {
            a.isUnlocked   = PlayerPrefs.GetInt($"Achievement_{a.achievementId}_Unlocked", 0) == 1;
            a.currentCount = PlayerPrefs.GetInt($"Achievement_{a.achievementId}_Count",    0);
        }
    }

    /// <summary>Resets all achievement progress (used when starting a new game).</summary>
    public void ResetAll()
    {
        foreach (AchievementData a in achievements)
        {
            a.isUnlocked   = false;
            a.currentCount = 0;
        }
        SaveProgress();
    }
}
