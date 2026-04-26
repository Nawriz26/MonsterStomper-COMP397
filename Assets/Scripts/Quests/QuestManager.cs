using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Observer that manages all quests.
/// Subscribes to the GameEventBus and advances quest steps accordingly.
/// The tutorial quest auto-starts when the game begins.
/// </summary>
public class QuestManager : MonoBehaviour, IGameObserver
{
    public static QuestManager Instance { get; private set; }

    [Header("Quests")]
    [Tooltip("All quests in the game. The first quest is used as the tutorial.")]
    [SerializeField] private List<QuestData> quests = new List<QuestData>();

    [Header("UI")]
    [SerializeField] private QuestUI questUI;

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

        ResetAllQuests();
    }

    void Start()
    {
        // Register in Start (not OnEnable) so GameEventBus.Instance is guaranteed
        // to be set after all Awake calls have completed.
        if (GameEventBus.Instance != null)
            GameEventBus.Instance.RegisterObserver(this);
        else
            Debug.LogWarning("QuestManager: GameEventBus instance not found during Start.");

        // Auto-start the tutorial (first quest) when gameplay begins
        if (quests.Count > 0)
            StartQuest(quests[0]);
    }

    void OnDestroy()
    {
        if (GameEventBus.Instance != null)
            GameEventBus.Instance.UnregisterObserver(this);
    }

    // ── IGameObserver ─────────────────────────────────────────────────────────

    public void OnNotify(GameEvent gameEvent, object data)
    {
        foreach (QuestData quest in quests)
        {
            if (quest.status != QuestStatus.Active) continue;

            QuestStep currentStep = quest.GetCurrentStep();
            if (currentStep == null || currentStep.triggerEvent != gameEvent) continue;

            bool stepJustCompleted = currentStep.RegisterProgress();

            questUI?.RefreshCurrentStep(quest);

            if (stepJustCompleted)
                OnStepCompleted(quest);
        }
    }

    // ── Quest Flow ────────────────────────────────────────────────────────────

    /// <summary>Activates a quest and notifies the UI.</summary>
    public void StartQuest(QuestData quest)
    {
        if (quest == null || quest.status != QuestStatus.Inactive) return;

        quest.status = QuestStatus.Active;
        Debug.Log($"QuestManager: Started quest — {quest.questTitle}");

        GameEventBus.Raise(GameEvent.QuestStarted, quest);
        questUI?.ShowQuest(quest);
    }

    private void OnStepCompleted(QuestData quest)
    {
        Debug.Log($"QuestManager: Step {quest.currentStep} complete in '{quest.questTitle}'");
        GameEventBus.Raise(GameEvent.QuestStepCompleted, quest);

        quest.currentStep++;

        bool questFinished = quest.currentStep >= quest.steps.Count;

        if (questFinished)
        {
            CompleteQuest(quest);
        }
        else
        {
            // Advance to the next step
            questUI?.ShowQuest(quest);
        }
    }

    private void CompleteQuest(QuestData quest)
    {
        quest.status = QuestStatus.Completed;
        Debug.Log($"QuestManager: Quest completed — {quest.questTitle}");

        GameEventBus.Raise(GameEvent.QuestCompleted, quest);
        questUI?.OnQuestCompleted(quest);

        // Auto-start the next inactive quest, if any
        QuestData nextQuest = quests.Find(q => q.status == QuestStatus.Inactive);
        if (nextQuest != null)
        {
            StartQuest(nextQuest);
        }
        else
        {
            questUI?.HideQuestPanel();
        }
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    public List<QuestData> GetAllQuests()         => quests;
    public QuestData       GetActiveQuest()        => quests.Find(q => q.status == QuestStatus.Active);
    public bool            IsQuestComplete(string questId)
        => quests.Exists(q => q.questId == questId && q.status == QuestStatus.Completed);

    // ── Reset ─────────────────────────────────────────────────────────────────

    private void ResetAllQuests()
    {
        foreach (QuestData q in quests)
            q.ResetRuntime();
    }
}
