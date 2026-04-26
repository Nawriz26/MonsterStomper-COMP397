using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// In-game quest panel that shows the current quest title and the steps as checkboxes.
/// Attach to the Quest Panel GameObject in the HUD Canvas
/// </summary>
public class QuestUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;

    [Header("Step Rows")]
    [Tooltip("Prefab with a Toggle (checkbox) and a TextMeshPro label.")]
    [SerializeField] private GameObject stepRowPrefab;
    [SerializeField] private Transform  stepsContainer;

    [Header("Completion Banner")]
    [SerializeField] private GameObject completionBanner;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private float completionDisplayDuration = 2.5f;

    private readonly List<GameObject> stepRows = new List<GameObject>();

    void Awake()
    {
        if (questPanel != null)    questPanel.SetActive(false);
        if (completionBanner != null) completionBanner.SetActive(false);
    }

    // ── Public API (called by QuestManager) ───────────────────────────────────

    /// <summary>Activates the panel and populates it with the given quest's steps.</summary>
    public void ShowQuest(QuestData quest)
    {
        if (quest == null) return;

        questPanel?.SetActive(true);

        if (questTitleText != null)       questTitleText.text       = quest.questTitle;
        if (questDescriptionText != null) questDescriptionText.text = quest.questDescription;

        BuildStepRows(quest);
    }

    /// <summary>Updates the checkbox state of the current step without rebuilding all rows.</summary>
    public void RefreshCurrentStep(QuestData quest)
    {
        if (quest == null) return;

        for (int i = 0; i < stepRows.Count && i < quest.steps.Count; i++)
        {
            QuestStep step   = quest.steps[i];
            Toggle    toggle = stepRows[i].GetComponentInChildren<Toggle>();
            if (toggle != null)
                toggle.isOn = step.isCompleted;

            TextMeshProUGUI label = stepRows[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = step.requiredCount > 1
                    ? $"{step.description} ({step.currentCount}/{step.requiredCount})"
                    : step.description;
            }
        }
    }

    /// <summary>Shows the completion banner then hides the quest panel.</summary>
    public void OnQuestCompleted(QuestData quest)
    {
        StartCoroutine(ShowCompletionBanner(quest.questTitle));
    }

    public void HideQuestPanel()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
    }

    // ── Internal Helpers ──────────────────────────────────────────────────────

    private void BuildStepRows(QuestData quest)
    {
        // Clear old rows
        foreach (GameObject row in stepRows)
            Destroy(row);
        stepRows.Clear();

        if (stepsContainer == null || stepRowPrefab == null) return;

        for (int i = 0; i < quest.steps.Count; i++)
        {
            QuestStep step = quest.steps[i];
            GameObject row = Instantiate(stepRowPrefab, stepsContainer);
            stepRows.Add(row);

            Toggle toggle = row.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.isOn          = step.isCompleted;
                toggle.interactable  = false; // display only
            }

            TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = step.requiredCount > 1
                    ? $"{step.description} ({step.currentCount}/{step.requiredCount})"
                    : step.description;
            }
        }
    }

    private IEnumerator ShowCompletionBanner(string questTitle)
    {
        if (completionBanner != null)
        {
            if (completionText != null)
                completionText.text = $"Quest Complete!\n{questTitle}";

            completionBanner.SetActive(true);
            yield return new WaitForSecondsRealtime(completionDisplayDuration);
            completionBanner.SetActive(false);
        }

        HideQuestPanel();
    }
}
