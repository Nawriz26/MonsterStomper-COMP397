using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private HashSet<string> unlockedAchievements = new HashSet<string>();

    public event Action<string> OnAchievementUnlocked;

    private void Awake()
    {
        Instance = this;
    }

    public void Unlock(string achievementID)
    {
        if (unlockedAchievements.Contains(achievementID))
            return;

        unlockedAchievements.Add(achievementID);

        Debug.Log("Achievement Unlocked: " + achievementID);

        OnAchievementUnlocked?.Invoke(achievementID);
    }
}