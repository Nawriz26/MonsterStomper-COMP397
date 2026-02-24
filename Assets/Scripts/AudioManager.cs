using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip optionsMusic;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip buttonHoverSFX;

    [Header("Gameplay Sound Effects")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip enemyHitSFX;
    [SerializeField] private AudioClip enemyDeathSFX;
    [SerializeField] private AudioClip playerHitSFX;
    [SerializeField] private AudioClip coinCollectSFX;
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] private AudioClip jumpSFX;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadVolumeSettings();
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSFX);
    }

    public void PlayShoot()
    {
        PlaySFX(shootSFX);
    }

    public void PlayEnemyHit()
    {
        PlaySFX(enemyHitSFX);
    }

    public void PlayEnemyDeath()
    {
        PlaySFX(enemyDeathSFX);
    }

    public void PlayPlayerHit()
    {
        PlaySFX(playerHitSFX);
    }

    public void PlayCoinCollect()
    {
        PlaySFX(coinCollectSFX);
    }

    public void PlayFootstep()
    {
        PlaySFX(footstepSFX);
    }

    public void PlayJump()
    {
        PlaySFX(jumpSFX);
    }

    public void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "GamePlay":
                PlayMusic(gameplayMusic);
                break;
            case "GameOver":
                PlayMusic(gameOverMusic);
                break;
            case "Options":
                PlayMusic(optionsMusic);
                break;
        }
    }
}
