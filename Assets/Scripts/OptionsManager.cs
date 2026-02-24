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
    }

    // ---------------- AUDIO ----------------

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // ---------------- GRAPHICS ----------------

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    // ---------------- LOAD ----------------

    void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        int quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = master;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = music;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfx;
        if (qualityDropdown != null)
            qualityDropdown.value = quality;

        AudioListener.volume = master;
        QualitySettings.SetQualityLevel(quality);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(music);
            AudioManager.Instance.SetSFXVolume(sfx);
        }
    }

    // ---------------- BACK BUTTON ----------------

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}