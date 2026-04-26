using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a full quest with one or more sequential steps.
/// Create via: Assets → Create → MonsterStomper → Quest Data
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "MonsterStomper/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Identity")]
    public string questId;
    public string questTitle;
    [TextArea(2, 4)]
    public string questDescription;
    public Sprite questIcon;

    [Header("Steps")]
    public List<QuestStep> steps = new List<QuestStep>();

    [Header("State (runtime)")]
    [HideInInspector] public QuestStatus status       = QuestStatus.Inactive;
    [HideInInspector] public int         currentStep  = 0;

    /// <summary>Returns the active step, or null if quest is finished.</summary>
    public QuestStep GetCurrentStep()
    {
        if (currentStep < steps.Count)
            return steps[currentStep];
        return null;
    }

    public bool IsCompleted => status == QuestStatus.Completed;

    /// <summary>Resets runtime state so the quest can be replayed after New Game.</summary>
    public void ResetRuntime()
    {
        status      = QuestStatus.Inactive;
        currentStep = 0;
        foreach (QuestStep step in steps)
        {
            step.isCompleted  = false;
            step.currentCount = 0;
        }
    }
}

public enum QuestStatus
{
    Inactive,
    Active,
    Completed
}
