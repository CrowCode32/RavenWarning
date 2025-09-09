using UnityEngine;

/// <summary>
/// Manages the overall game state, player data, and major events like the storm.
/// Implemented as a singleton to be easily accessible from other scripts.
/// </summary>

public class GameManager : MonoBehaviour
{
    // A static instance of the GameManager to be accessed from anywhere.
    public static GameManager instance;

    [Header("Object References")]
    [Tooltip("Assign the player's GameObject here.")]
    public GameObject player;

    [Tooltip("Assign the main Journal UI Panel here.")]
    public GameObject journalMenuUI;


    private void Awake()
    {
        // --- Singleton Pattern Implementation ---
        // If an instance already exists and it's not this one, destroy this new one.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // This is the first instance. Make it the singleton and ensure it persists
        // between scene loads (e.g., when returning to the hub).
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

        private void Start()
    {
        // Ensure the journal is closed at the start of the game.
        if (journalMenuUI != null)
        {
            journalMenuUI.SetActive(false);
        }
    }
}