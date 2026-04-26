using UnityEngine;

/// <summary>
/// ScriptableObject that defines a single achievement.
/// Create instances via: Assets → Create → MonsterStomper → Achievement Data
/// </summary>
[CreateAssetMenu(fileName = "Achievement", menuName = "MonsterStomper/Achievement Data")]
public class AchievementData : ScriptableObject
{
    [Header("Identity")]
    public string achievementId;
    public string title;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("Unlock Condition")]
    public GameEvent triggerEvent;
    public int requiredCount = 1;

    [Header("State (runtime)")]
    [HideInInspector] public bool isUnlocked = false;
    [HideInInspector] public int  currentCount = 0;
}
