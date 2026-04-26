using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Renders all achievements in a scrollable panel.
/// Attach to a Content Transform inside a Scroll View.
/// </summary>
public class AchievementListUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject achievementRowPrefab;
    [SerializeField] private Transform  contentParent;
    [SerializeField] private GameObject achievementListPanel;

    private bool isOpen = false;

    void Start()
    {
        if (achievementListPanel != null)
            achievementListPanel.SetActive(false);

        RefreshList();
    }

    /// <summary>Toggles the achievement list panel visibility.</summary>
    public void TogglePanel()
    {
        isOpen = !isOpen;
        if (achievementListPanel != null)
            achievementListPanel.SetActive(isOpen);

        if (isOpen)
            RefreshList();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }

    /// <summary>Rebuilds the achievement list from AchievementManager's data.</summary>
    public void RefreshList()
    {
        if (AchievementManager.Instance == null || contentParent == null) return;

        // Clear existing rows
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        List<AchievementData> achievements = AchievementManager.Instance.GetAllAchievements();

        foreach (AchievementData achievement in achievements)
        {
            GameObject row = Instantiate(achievementRowPrefab, contentParent);
            SetupRow(row, achievement);
        }
    }

    private void SetupRow(GameObject row, AchievementData achievement)
    {
        // Icon
        Image icon = row.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite  = achievement.icon;
            icon.enabled = achievement.icon != null;
        }

        // Title
        TextMeshProUGUI title = row.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
        if (title != null)
            title.text = achievement.title;

        // Description
        TextMeshProUGUI desc = row.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
        if (desc != null)
            desc.text = achievement.isUnlocked ? achievement.description : "???";

        // Lock indicator
        GameObject lockIndicator = row.transform.Find("LockOverlay")?.gameObject;
        if (lockIndicator != null)
            lockIndicator.SetActive(!achievement.isUnlocked);

        // Progress text
        TextMeshProUGUI progress = row.transform.Find("Progress")?.GetComponent<TextMeshProUGUI>();
        if (progress != null)
        {
            progress.text = achievement.isUnlocked
                ? "Unlocked!"
                : $"{achievement.currentCount}/{achievement.requiredCount}";
        }
    }
}
