using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Global Data")]
    public PlayerProgress playerProgress;
    public LevelData currentLevel;

    [Header("Managers")]
    public AudioManager audioManager;
    public SaveLoadManager saveLoadManager;
    public ObjectPoolManager objectPoolManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    // Initializes core subsystems required across all scenes.
    private void InitializeManagers()
    {
        audioManager = GetComponentInChildren<AudioManager>();
        saveLoadManager = GetComponentInChildren<SaveLoadManager>();
        objectPoolManager = GetComponentInChildren<ObjectPoolManager>();

        if (saveLoadManager != null)
            playerProgress = saveLoadManager.LoadProgress();
    }

    // Loads a specified scene by name.
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Starts a gameplay level using the provided LevelData.
    public void StartLevel(LevelData levelData)
    {
        currentLevel = levelData;
        SceneManager.LoadScene(levelData.sceneName);
    }

    // Called by LevelManager when a level completes (all enemies defeated).
    // For now: save progress and return to Hub. Expand this to award XP/money UI as needed.
    public void OnLevelCompleted()
    {
        Debug.Log("GameManager: Level completed.");

        // TODO: compute XP/money awards here (hook into XPManager / EconomyManager later).

        // Save progress
        if (saveLoadManager != null && playerProgress != null)
            saveLoadManager.SaveProgress(playerProgress);

        // Return to hub (or show results UI)
        LoadScene("Hub");
    }

    // Returns to the main hub scene.
    public void ReturnToHub()
    {
        SceneManager.LoadScene("Hub");
    }

    // Saves current player progress.
    public void SaveGame()
    {
        if (saveLoadManager != null && playerProgress != null)
            saveLoadManager.SaveProgress(playerProgress);
    }
}
