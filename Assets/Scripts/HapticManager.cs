using UnityEngine;

/// <summary>
/// Centralises haptic feedback (vibration) for mobile platforms.
/// All haptic calls go through this singleton so they respect the player's
/// haptic toggle in the Options screen.
/// On non-mobile platforms the calls are silently ignored.
/// </summary>
public class HapticManager : MonoBehaviour
{
    private const string HapticPrefKey = "HapticsEnabled";

    public static HapticManager Instance { get; private set; }

    /// <summary>Whether haptic feedback is currently enabled.</summary>
    public bool HapticsEnabled { get; private set; } = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HapticsEnabled = PlayerPrefs.GetInt(HapticPrefKey, 1) == 1;
    }

    /// <summary>Enables or disables haptic feedback and persists the preference.</summary>
    public void SetHapticsEnabled(bool enabled)
    {
        HapticsEnabled = enabled;
        PlayerPrefs.SetInt(HapticPrefKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Triggers a short vibration on supported mobile devices.</summary>
    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!HapticsEnabled) return;
        Handheld.Vibrate();
#endif
    }

    // ── Semantic helpers ──────────────────────────────────────────

    /// <summary>Called when the player takes damage.</summary>
    public void OnPlayerHit() => Vibrate();

    /// <summary>Called when the player jumps and lands.</summary>
    public void OnJump() => Vibrate();

    /// <summary>Called when a coin or collectible is picked up.</summary>
    public void OnCollect() => Vibrate();

    /// <summary>Called when an enemy is killed.</summary>
    public void OnEnemyKilled() => Vibrate();
}
