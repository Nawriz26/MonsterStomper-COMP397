using UnityEngine;

/// <summary>
/// Represents a single step within a quest.
/// A step is complete when currentCount reaches requiredCount.
/// </summary>
[System.Serializable]
public class QuestStep
{
    public string stepId;
    public string description;

    [Header("Completion Condition")]
    public GameEvent triggerEvent;
    public int       requiredCount = 1;

    [Header("State (runtime)")]
    public bool isCompleted  = false;
    public int  currentCount = 0;

    /// <summary>Increments the counter and returns true if the step just completed.</summary>
    public bool RegisterProgress()
    {
        if (isCompleted) return false;

        currentCount++;
        if (currentCount >= requiredCount)
        {
            isCompleted = true;
            return true;
        }
        return false;
    }

    public float GetProgress() =>
        requiredCount > 0 ? (float)currentCount / requiredCount : 0f;
}
