using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the How To Play panel in the Main Menu.
/// Cycles through a set of tutorial slides and handles open/close.
/// </summary>
public class HowToPlayPanel : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Slides")]
    [SerializeField] private GameObject[] slides;

    [Header("Navigation")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI pageIndicatorText;

    [Header("Close")]
    [SerializeField] private Button closeButton;

    private int currentSlideIndex = 0;

    private const string PageIndicatorFormat = "{0} / {1}";

    void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (previousButton != null)
            previousButton.onClick.AddListener(ShowPreviousSlide);

        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNextSlide);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    /// <summary>Opens the panel and resets to the first slide.</summary>
    public void Open()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        currentSlideIndex = 0;
        if (panelRoot != null)
            panelRoot.SetActive(true);

        RefreshSlides();
    }

    /// <summary>Closes the panel.</summary>
    public void Close()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ShowPreviousSlide()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            RefreshSlides();
        }
    }

    private void ShowNextSlide()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        if (currentSlideIndex < slides.Length - 1)
        {
            currentSlideIndex++;
            RefreshSlides();
        }
    }

    private void RefreshSlides()
    {
        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i] != null)
                slides[i].SetActive(i == currentSlideIndex);
        }

        if (pageIndicatorText != null)
            pageIndicatorText.text = string.Format(PageIndicatorFormat, currentSlideIndex + 1, slides.Length);

        if (previousButton != null)
            previousButton.interactable = currentSlideIndex > 0;

        if (nextButton != null)
            nextButton.interactable = currentSlideIndex < slides.Length - 1;
    }
}
