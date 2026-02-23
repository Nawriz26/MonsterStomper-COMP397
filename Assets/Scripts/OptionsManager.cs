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

        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
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

        masterVolumeSlider.value = master;
        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;
        qualityDropdown.value = quality;

        AudioListener.volume = master;
        QualitySettings.SetQualityLevel(quality);
    }

    // ---------------- BACK BUTTON ----------------

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}