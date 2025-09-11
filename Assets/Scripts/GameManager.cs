using System.Collections;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Manages the overall game state, player data, and major events like the storm.
/// Implemented as a singleton to be easily accessible from other scripts.
/// </summary>

public class GameManager : MonoBehaviour
{
    // A static instance of the GameManager to be accessed from anywhere.
    public static GameManager instance;

    private GameData gameData;
    private string saveFilePath;

    [Header("Object References")]
    [Tooltip("Assign the player's GameObject here.")]
    public GameObject player;

    [Tooltip("Assign the main Journal UI Panel here.")]
    public GameObject journalMenuUI;
    [Tooltip("Assign the main Settings UI Panel here.")]
    public GameObject SettingsMenuUI;
    [Tooltip("Assign the main Stats UI Panel here.")]
    public GameObject StatsMenuUI;
    [SerializeField] TMP_Dropdown featherDrop;
    [SerializeField] TMP_Dropdown trinketDrop;


    [Tooltip("Assign the main Unlocks UI Panel here.")]
    public GameObject UnlocksMenuUI;

    [Tooltip("Assign the scene's main camera here.")]
    public Camera mainCamera;

    [Header("Storm Mechanics")]
    [Tooltip("The storm wall prefab that will chase the player.")]
    public GameObject stormPrefab;

    [Tooltip("An empty GameObject marking the storm's initial spawn position.")]
    public Transform stormSpawnPoint;

    [Tooltip("An empty GameObject marking the storm's last possible position.")]
    public Transform stormEndPoint;

    [Tooltip("The delay in seconds after leaving the tutorial before the storm spawns.")]
    public float stormSpawnDelay = 120.0f; // Defaulting to 2 minutes (120s)


    [Header("Feather")]
    [Tooltip("Updates featherQueue.")]
    public feather selectedFeather;
    [Tooltip("Acquired feathers.")]
    public List<feather> feathersAquired = new List<feather>();

    [Header("Trinket")]
    [Tooltip("Updates player trinket.")]
    public trinket selectedTrinket;
    [Tooltip("Acquired trinkets.")]
    public List<trinket> trinketsAquired = new List<trinket>();

    int featherIndex;
    int trinketIndex;

    private bool isJournalOpen = false;
    private bool hasRunStarted = false;

    public bool isPaused;
    float timeScaleOrig;


    private void Awake()
    {
        // A safe place to store player data.
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");

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

        timeScaleOrig = Time.timeScale;

        DontDestroyOnLoad(gameObject);

        // Load the game as soon as the manager is ready
        LoadGame();
    }

    private void Start()
    {
        UpdateTrinketDropdown();
        UpdateFeatherDropdown();
        // Ensure the journal is closed at the start of the game.
        if (journalMenuUI != null)
        {
            journalMenuUI.SetActive(false);
        }

        // If a camera hasn't been assigned manually, find it.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
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

        // A temporary way to test saving the game.
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
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
            Instantiate(stormPrefab, stormSpawnPoint.transform);
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

    // <summary>
    /// Loads a new scene by its string name.
    /// Make sure the scene is added to the Build Settings.
    /// </summary>
    /// <param name="sceneName">The name of the scene file to load.</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateTrinketDropdown()
    {
        trinketDrop.ClearOptions();
        List<string> trinketNames = new List<string>();
        trinketNames.Add("None");
        for (int i = 0; i < trinketsAquired.Count; i++)
        {
            trinketNames.Add(trinketsAquired[i].trinketName);
        }

        trinketDrop.AddOptions(trinketNames);
    }
    public void UpdateFeatherDropdown()
    {
        featherDrop.ClearOptions();
        List<string> featherNames = new List<string>();
        featherNames.Add("None");
        for (int i = 0; i < feathersAquired.Count; i++)
        {
            featherNames.Add(feathersAquired[i].featherName);
        }

        featherDrop.AddOptions(featherNames);

    }

    public void OnFeatherDropdownChanged()
    {

        featherIndex = featherDrop.value;

        if (featherIndex == 0)
        {
            selectedFeather = null;
        }
        else
        {
            selectedFeather = feathersAquired[featherIndex - 1];
        }


    }

    public void OnTrinketDropdownChanged()
    {

        trinketIndex = trinketDrop.value;

        if (trinketIndex == 0)
        {
            selectedTrinket = null;
        }
        else
        {
            selectedTrinket = trinketsAquired[trinketIndex - 1];
        }


    }

    /// <summary>
    /// Saves the current game data to a JSON file.
    /// </summary>

    public void SaveGame()
    {
        // For testing, we'll just add 10 currency each time we save.
        gameData.currency += 10;

        // Convert the GameData object to a JSON string.
        string json = JsonUtility.ToJson(gameData, true);

        // Write the JSON string to the file.
        File.WriteAllText(saveFilePath, json);

        // Use a log that confirms the value that was saved.
        Debug.Log("Game data saved! Current currency: " + gameData.currency);
    }

    /// <summary>
    /// Loads game data from a JSON file, or creates a new game if no file exists.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            // If a save file exists, read it.
            string json = File.ReadAllText(saveFilePath);

            // Convert the JSON string back to a GameData object.
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game data loaded from: " + saveFilePath);
        }
        else
        {
            // If no save file exists, create a new GameData object with default values.
            Debug.Log("No save file found. Creating a new game.");
            gameData = new GameData();
        }
    }

    public void lordInformed(string lord)
    {
        GameData.instance.inform(lord);
        Debug.Log("Informed");
    }
}