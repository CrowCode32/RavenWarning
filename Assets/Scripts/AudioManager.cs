using UnityEngine;

// <summary>
/// Manages all audio playback for the game, including background music and sound effects.
/// Implemented as a singleton to be easily accessible and persist across scenes.
/// </summary>

public class AudioManager : MonoBehaviour
{
    // A static instance of the AudioManager for easy access.
    public static AudioManager instance;

    [Header("Audio Sources")]
    [Tooltip("The AudioSource component for playing background music (BGM).")]
    public AudioSource bgmSource;

    // We can add another AudioSource for sound effects (SFX) later.
    // public AudioSource sfxSource;

    private void Awake()
    {
        // --- Singleton Pattern Implementation ---
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays a new background music track.
    /// </summary>
    /// <param name="musicClip">The audio clip to play.</param>

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        bgmSource.clip = musicClip;
        bgmSource.Play();
    }
}
