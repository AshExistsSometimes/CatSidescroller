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
        // Enforce Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize Core Managers
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