using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays a brief toast notification when an achievement is unlocked.
/// Attach to a Canvas panel that slides in/out from a corner of the screen.
/// </summary>
public class AchievementNotificationUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private Image      achievementIcon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float slideDuration   = 0.3f;
    [SerializeField] private Vector2 hiddenAnchoredPos  = new Vector2(400f, 0f);
    [SerializeField] private Vector2 visibleAnchoredPos = new Vector2(0f,   0f);

    private RectTransform panelRect;
    private Queue<AchievementData> notificationQueue = new Queue<AchievementData>();
    private bool isShowing = false;

    void Awake()
    {
        // Resolve panelRect while the GO is still active (before any SetActive(false)).
        // notificationPanel is the root of this component's own GameObject, so
        // GetComponent is always safe here regardless of child active state.
        if (notificationPanel != null)
            panelRect = notificationPanel.GetComponent<RectTransform>();

        // Snap to the hidden position before deactivating so the first slide-in
        // always starts from off-screen rather than from wherever Unity placed it.
        if (panelRect != null)
            panelRect.anchoredPosition = hiddenAnchoredPos;

        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    /// <summary>Enqueues a notification; shows immediately if none is active.</summary>
    public void ShowNotification(AchievementData achievement)
    {
        notificationQueue.Enqueue(achievement);
        if (!isShowing)
            StartCoroutine(ShowNextInQueue());
    }

    private IEnumerator ShowNextInQueue()
    {
        while (notificationQueue.Count > 0)
        {
            AchievementData achievement = notificationQueue.Dequeue();
            yield return StartCoroutine(DisplayNotification(achievement));
        }
        isShowing = false;
    }

    private IEnumerator DisplayNotification(AchievementData achievement)
    {
        isShowing = true;

        // Populate UI
        if (titleText != null)       titleText.text       = achievement.title;
        if (descriptionText != null) descriptionText.text = achievement.description;
        if (achievementIcon != null)
        {
            achievementIcon.sprite  = achievement.icon;
            achievementIcon.enabled = achievement.icon != null;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        // Slide in
        notificationPanel.SetActive(true);
        yield return StartCoroutine(SlidePanel(hiddenAnchoredPos, visibleAnchoredPos));

        // Hold
        yield return new WaitForSecondsRealtime(displayDuration);

        // Slide out
        yield return StartCoroutine(SlidePanel(visibleAnchoredPos, hiddenAnchoredPos));

        notificationPanel.SetActive(false);
    }

    private IEnumerator SlidePanel(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            panelRect.anchoredPosition = Vector2.Lerp(from, to, elapsed / slideDuration);
            yield return null;
        }
        panelRect.anchoredPosition = to;
    }
}
