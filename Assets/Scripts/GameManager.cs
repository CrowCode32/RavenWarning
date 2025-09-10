using UnityEngine;
using System.Collections;

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

    [Header("Storm Mechanics")]
    [Tooltip("The storm wall prefab that will chase the player.")]
    public GameObject stormPrefab;

    [Tooltip("An empty GameObject marking the storm's initial spawn position.")]
    public Transform stormSpawnPoint;

    [Tooltip("The delay in seconds after leaving the tutorial before the storm spawns.")]
    public float stormSpawnDelay = 120.0f; // Defaulting to 2 minutes (120s)

    private bool isJournalOpen = false;
    private bool hasRunStarted = false;


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

    private void Update()
    {
        // Check for the journal input key (e.g., 'J' or 'Tab').
        if (Input.GetKeyDown(KeyCode.J))
        {
            // This will print a message to the console every time we press 'J'.
            Debug.Log("'J' key pressed!");
            ToggleJournal();
        }
    }

    /// <summary>
    /// This method should be called by a trigger when the player exits the tutorial area.
    /// </summary>

    public void BeginRun()
    {
        // Ensure this can only be called once per run.
        if (!hasRunStarted)
        {
            hasRunStarted = true;
            Debug.Log("Run has started! Storm timer initiated.");
            // The run has officially started, so we begin the storm countdown.
            StartCoroutine(SpawnStormCoroutine());
        }
    }

    private IEnumerator SpawnStormCoroutine()
    {
        // Wait for the specified delay to give the player a head start.
        yield return new WaitForSeconds(stormSpawnDelay);

        Debug.Log("Spawning the storm!");
        // Now, spawn the storm.
        if (stormPrefab != null && stormSpawnPoint != null)
        {
            Instantiate(stormPrefab, stormSpawnPoint.position, stormSpawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("GameManager is missing the Storm Prefab or Storm Spawn Point reference!");
        }
    }

    /// <summary>
    /// Toggles the journal UI open and closed.
    /// This can be called from a UI Button's OnClick() event.
    /// </summary>

    public void ToggleJournal()
    {
        isJournalOpen = !isJournalOpen;
        journalMenuUI.SetActive(isJournalOpen);
    }
}