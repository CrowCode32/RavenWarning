using UnityEngine;

/// <summary>
/// Manages the playback of all sound effects (SFX) in the game.
/// Implemented as a singleton to be easily accessible from any script.
/// </summary>
public class SoundEffectManager : MonoBehaviour
{
    // A static instance of the SoundEffectManager for easy access.
    public static SoundEffectManager instance;

    [Header("Audio Source")]
    [Tooltip("The AudioSource component responsible for playing all SFX.")]
    [SerializeField] private AudioSource sfxSource;

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
    /// Plays a given audio clip one time.
    /// </summary>
    /// <param name="clip">The sound effect to play.</param>
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // PlayOneShot allows multiple sounds to be played without cutting each other off.
            sfxSource.PlayOneShot(clip);
        }
    }
}