using UnityEngine;

/// <summary>
/// A simple component that tells the AudioManager what music to play when a scene starts.
/// </summary>

public class MusicPlayer : MonoBehaviour
{
    [Header("Scene Music")]
    [Tooltip("The background music that should play in this scene.")]
    public AudioClip sceneMusic;

    void Start()
    {
        // Find the AudioManager instance and tell it to play our designated music clip.
        if (AudioManager.instance != null && sceneMusic != null)
        {
            AudioManager.instance.PlayMusic(sceneMusic);
        }
    }
}
