using System.Collections;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Threading.Tasks;



/// <summary>
/// Manages the overall game state, player data, and major events like the storm.
/// Implemented as a singleton to be easily accessible from other scripts.
/// </summary>

public class GameManager : MonoBehaviour
{
    // A static instance of the GameManager to be accessed from anywhere.
    public static GameManager instance;

    public bool firstLoad = true;

    private GameData gameData;
    private string saveFilePath;

    [Header("Object References")]
    [Tooltip("Assign the player's GameObject here.")]
    public GameObject player;

    [Tooltip("Assign the player's Healthbar here.")]
    public Image playerHP;


    [Tooltip("The currently active menu.")]
    public GameObject activeMenu;

    [Tooltip("Assign the main Menu UI Panel here.")]
    public GameObject mainMenuUI;

    [Tooltip("Assign the player HUD here.")]
    public GameObject playerHUD;

    [Tooltip("Assign the Loading screen UI Panel here.")]
    public GameObject loadingScreenUI;
    [SerializeField] Image loadingBar;

    [Tooltip("Assign the Pause Menu UI Panel here.")]
    public GameObject pauseMenuUI;

    [Tooltip("Assign the main Journal UI Panel here.")]
    public GameObject journalMenuUI;

    [Tooltip("Three journal UI menus")]
    public List<GameObject> journalMenus;
    [Tooltip("Next button.")] 
    public GameObject nextButton;
    [Tooltip("Prev button.")]
    public GameObject prevButton;

    [SerializeField] public Slider masterVolumeSlider;
    [SerializeField] public Slider musicVolumeSlider;
    [SerializeField] public Slider SFXVolumeSlider;
    [SerializeField] public Slider mouseSensitivitySlider;
    [SerializeField] public Slider brightnessSlider;
    [SerializeField] public Image brightnessImage;


    [Tooltip("Press any key...")]
    public GameObject keyInput;

   

    public float mouseSensitivity = 1f;


    public Image progFill;
    public Image playerIcon;
    public Image stormIcon;
    public GameObject dialogueBox;

    [SerializeField] TMP_Dropdown featherDrop;
    [SerializeField] TMP_Dropdown trinketDrop;
    


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
    
    [Tooltip("A value offsetting the storm once the player changes levels based on how far ahead of it they were.")]
    public float stormOffset;

    [Header("Run Progress")]
    [Tooltip("Tracks if the player has informed the lord in the cave level.")]
    public bool lordInCaveInformed;
    [Tooltip("Tracks if the player has informed the lord in the forest level.")]
    public bool lordInForestInformed;
    public bool inGraveyard = false;
    public bool inCave = false;
    public bool inForest = false;
    public bool inKingdom = false;
    public bool inKing = false;

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

    [SerializeField] public List<TrinketSlotUI> trinketSlots;

    int featherIndex;
    int trinketIndex;
    public int journalMenuIndex = 0;

    private bool isJournalOpen = false;
    private bool hasRunStarted = false;

    public bool isPaused;
    float timeScaleOrig;
    public bool gameStarted = false;
    AsyncOperation currentLoad;



    private void Awake()
    {
        // Use the instance ID to uniquely identify each GameManager object.
        Debug.Log("GameManager Awake() called by object: " + gameObject.name + " (ID: " + GetInstanceID() + ")");

        // This path works on all platforms (Windows, Mac, Linux)
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");

        // --- Singleton Pattern Implementation ---
        if (instance != null && instance != this)
        {
            Debug.LogWarning("An instance of GameManager already exists (ID: " + instance.GetInstanceID() + "). Destroying this new one (ID: " + GetInstanceID() + ").");
            Destroy(gameObject);
            return;
        }

        instance = this;
        Debug.Log("This object is now the official GameManager instance (ID: " + GetInstanceID() + "). Setting DontDestroyOnLoad.");

        timeScaleOrig = Time.timeScale;

        DontDestroyOnLoad(gameObject);

        // Load the game as soon as the manager is ready
        LoadGame();

        if (firstLoad)
        {
            firstLoad = false;
            //activeMenu = mainMenuUI;
            //activeMenu.SetActive(true);
            statePause();
        }
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
            if(gameStarted)
            {
                ToggleJournal();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activeMenu = pauseMenuUI;
            activeMenu.SetActive(true);
            statePause();
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
        journalMenus[journalMenuIndex].SetActive(true);
        nextButton.SetActive(true);
        prevButton.SetActive(true);

        if(activeMenu != pauseMenuUI)
        {
            statePause();
        }
       

        if(isJournalOpen == false)
        {
            stateUnpause();
        }
    }

    /// <summary>
    /// Loads a new scene by its string name.
    /// Make sure the scene is added to the Build Settings.
    /// </summary>
    /// <param name="sceneName">The name of the scene file to load.</param>

    public void loadingScene(string scene)
    {
        if(activeMenu != null)
        {
            activeMenu.SetActive(false);
            activeMenu = null;
        }

        loadingScreenUI.SetActive(true);
        activeMenu = loadingScreenUI;
        StartCoroutine(loadLevelAsync(scene));
        Debug.Log("Loading...");
    }

    IEnumerator loadLevelAsync(string scene)
    {

        currentLoad = SceneManager.LoadSceneAsync(scene);

        while (!currentLoad.isDone)
        {
            float loadProg = Mathf.Clamp01(currentLoad.progress / 0.9f);
            loadingBar.fillAmount = loadProg;
            yield return null;
        }
        
        loadingScreenUI.SetActive(false);
        activeMenu = null;

        if(scene == "Forest" || scene == "Cave" || scene == "Kingdom")
        {
            StartCoroutine(stormSpawnReady());
        }

    }

    public void loadMainMenu()
    {
        loadingScene("MainMenu");
        //mainMenuUI.SetActive(true);
        //activeMenu =mainMenuUI;
        statePause();
    }
    
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
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
            activeMenu = null;
        }

        playerHUD.SetActive(true);
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }



    /// <summary>
    /// Saves the current game data to a JSON file.
    /// </summary>
 


    public float ApplySlider(Slider slider)
    {
        return slider.value;
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

    public void EnableJournalEntryTrinket(int num)
    {
        Debug.Log(num);
        TrinketSlotUI slot = trinketSlots[num];
        slot.locked.gameObject.SetActive(false);
        slot.unlocked.gameObject.SetActive(true);

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

    /// <summary>
    /// Sets the status of the Cave Lord to 'informed' and saves the game.
    /// </summary>
    public void InformLordOfCave()
    {
        // Only update and save if this is the first time informing this lord.
        if (!lordInCaveInformed)
        {
            lordInCaveInformed = true;
            Debug.Log("The lord in the cave has been informed.");
            SaveGame();
        }
    }

    /// <summary>
    /// Sets the status of the Forest Lord to 'informed' and saves the game.
    /// </summary>
      public void InformLordOfForest()
    {
        // Only update and save if this is the first time informing this lord.
        if (!lordInForestInformed)
        {
            lordInForestInformed = true;
            Debug.Log("The lord in the forest has been informed.");
            SaveGame();
        }
    }


    /// <summary>
    /// This is the central function for updating a lord's status.
    /// I set it up this way so any lord in the game can just call this one
    /// function instead of having its own logic.
    /// </summary>

    public void lordInformed(string lord)
    {
        // I'm keeping all the save data in the 'gameData' object, which the
        // GameManager creates and manages. This makes the GameManager our "single
        // source of truth" for all game progress. It's cleaner than making 
        // GameData a singleton because this keeps our data separate from our logic.

        // Check which lord is being referenced by the string
        if (lord == "Forest" && !gameData.lesserLordForestInformed)
        {
            // The GameManager is the only thing that should be allowed to change
            // the game's data. Then, it immediately saves the progress.
            gameData.lesserLordForestInformed = true;
            Debug.Log("The lord in the forest has been informed.");
            SaveGame(); 
        }
        else if (lord == "Cave" && !gameData.lesserLordCaveInformed)
        {
            gameData.lesserLordCaveInformed = true;
            Debug.Log("The lord in the cave has been informed.");
            SaveGame(); 
        }
    }

    // This method would be called when the player dies, resetting only values which are not maintained after death.
    public void deathDataReset()
    {
        gameData.lesserLordForestInformed = false;
        gameData.lesserLordCaveInformed = false;
    }

    // This method is called when the game is TRULY won.
    public void gameWon()
    {
        statePause();
        Debug.Log("Game won");

        loadingScene("Credits");
        Time.timeScale = 1;
        //Credits animation triggers main menu
    }

    public void gameLost()
    {
        Debug.Log("Called");
        statePause();
        Debug.Log("Game lost");

        loadingScene("Graveyard");
        loadMainMenu();
    }
    
    Vector2 findProgFill()
    {
        if (inKing) { progFill.fillAmount = 1f; }
        else if (inKingdom) { progFill.fillAmount = 0.6f; }
        else if (inForest) { progFill.fillAmount = 0.3f;  }
        else if (inCave) { progFill.fillAmount = 0.1f; }
        else if (inGraveyard) { progFill.fillAmount = .02f; }
            
        RectTransform fillTrans = progFill.rectTransform;
        Rect fillRect = fillTrans.rect;

        float fillAmount = progFill.fillAmount;

        // Location of fill edge locally
        float xPos = Mathf.Lerp(fillRect.xMin, fillRect.xMax, fillAmount);
        float yPos = fillRect.center.y;

        // Local pos as vector
        Vector2 localPos = new Vector2(xPos, yPos);

        // Converted to world & then screen pos
        Vector2 worldPos = fillTrans.TransformPoint(localPos);
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        Vector2 currPos = playerIcon.transform.position;
        return new Vector2(screenPos.x, currPos.y);
    }

    public void updateProgUI()
    {
        playerIcon.transform.position = findProgFill();
    }

    private IEnumerator stormSpawnReady()
    {
        while(currentLoad != null && !currentLoad.isDone)
        {
            yield return null;
        }

        loadStorm();
    }
    public async void loadStorm(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        Debug.Log("Print please");
        await asyncLoad;
    }


    public async void loadStorm()
    {
        updateProgUI();
        Debug.Log("Updated");
        if (stormPrefab != null && stormSpawnPoint != null)
        {
            Debug.Log("Spawned");
            Instantiate(stormPrefab, stormSpawnPoint.transform);
        }
        else
        {
            Debug.LogWarning("GameManager is missing the Storm Prefab or Storm Spawn Point reference!");
        }
    }
}