using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Graphics")]
    public TMP_Dropdown qualityDropdown;

    [Header("Mobile Controls")]
    [Tooltip("Slider (0.05 – 0.5) that controls touch swipe sensitivity in CameraController.")]
    public Slider cameraSensitivitySlider;
    [Tooltip("Toggle that inverts the touch look Y axis.")]
    public Toggle invertYToggle;
    [Tooltip("Toggle that enables or disables haptic feedback.")]
    public Toggle hapticsToggle;

    private const string SensitivityPrefKey = "CameraSensitivity";
    private const string InvertYPrefKey     = "InvertY";

    void Start()
    {
        LoadSettings();
        AddListeners();
    }

    private void AddListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        if (cameraSensitivitySlider != null)
            cameraSensitivitySlider.onValueChanged.AddListener(SetCameraSensitivity);
        if (invertYToggle != null)
            invertYToggle.onValueChanged.AddListener(SetInvertY);
        if (hapticsToggle != null)
            hapticsToggle.onValueChanged.AddListener(SetHaptics);
    }

    // ── AUDIO ────────────────────────────────────────────────────

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    // ── GRAPHICS ─────────────────────────────────────────────────

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    // ── MOBILE CONTROLS ──────────────────────────────────────────

    /// <summary>Updates touch camera sensitivity (0.05 – 0.5 range recommended).</summary>
    public void SetCameraSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityPrefKey, value);
        ApplySensitivityToControllers(value);
    }

    /// <summary>Inverts the touch look Y axis.</summary>
    public void SetInvertY(bool inverted)
    {
        PlayerPrefs.SetInt(InvertYPrefKey, inverted ? 1 : 0);
        ApplyInvertYToControllers(inverted);
    }

    /// <summary>Enables or disables haptic vibration feedback.</summary>
    public void SetHaptics(bool enabled)
    {
        if (HapticManager.Instance != null)
            HapticManager.Instance.SetHapticsEnabled(enabled);
    }

    // ── LOAD ─────────────────────────────────────────────────────

    void LoadSettings()
    {
        float master      = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music       = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx         = PlayerPrefs.GetFloat("SFXVolume", 1f);
        int   quality     = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        float sensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, 0.15f);
        bool  invertY     = PlayerPrefs.GetInt(InvertYPrefKey, 0) == 1;
        bool  haptics     = HapticManager.Instance == null || HapticManager.Instance.HapticsEnabled;

        if (masterVolumeSlider != null) masterVolumeSlider.value = master;
        if (musicVolumeSlider  != null) musicVolumeSlider.value  = music;
        if (sfxVolumeSlider    != null) sfxVolumeSlider.value    = sfx;
        if (qualityDropdown    != null) qualityDropdown.value    = quality;

        if (cameraSensitivitySlider != null) cameraSensitivitySlider.value = sensitivity;
        if (invertYToggle           != null) invertYToggle.isOn            = invertY;
        if (hapticsToggle           != null) hapticsToggle.isOn            = haptics;

        AudioListener.volume = master;
        QualitySettings.SetQualityLevel(quality);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(music);
            AudioManager.Instance.SetSFXVolume(sfx);
        }

        ApplySensitivityToControllers(sensitivity);
        ApplyInvertYToControllers(invertY);
    }

    private void ApplySensitivityToControllers(float value)
    {
        foreach (var controller in FindObjectsByType<MobileTouchController>(FindObjectsSortMode.None))
            controller.SetSensitivity(value);
    }

    private void ApplyInvertYToControllers(bool inverted)
    {
        foreach (var controller in FindObjectsByType<MobileTouchController>(FindObjectsSortMode.None))
            controller.SetInvertY(inverted);
    }

    // ── BACK ─────────────────────────────────────────────────────

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}