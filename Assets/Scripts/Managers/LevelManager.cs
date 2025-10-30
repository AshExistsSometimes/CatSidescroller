using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data Reference")]
    public LevelData currentLevelData;

    [Header("Spawn Settings")]
    public Transform enemySpawnPoint;  // spawn X position (off-screen right)
    public Transform groundReference;  // Y position considered ground (Y = 0)
    public float topOfScreenY = 5f;    // world Y for enemyHeight == 1

    [Header("Spawned Enemies (Debug)")]
    public List<GameObject> activeEnemies = new List<GameObject>();

    private bool levelRunning = false;
    private bool bossActive = false;
    private bool levelComplete = false;

    [Header("UI")]
    public GameObject ResultsScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        // Wait one frame to ensure GameManager and other singletons are initialized
        yield return null;

        if (GameManager.Instance != null && GameManager.Instance.currentLevel != null)
        {
            Debug.Log($"LevelManager: Found LevelData from GameManager ({GameManager.Instance.currentLevel.levelID})");
            BeginLevel(GameManager.Instance.currentLevel);
            ResultsScreen.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LevelManager: No LevelData found in GameManager. Using fallback LevelData if assigned manually.");
            if (currentLevelData != null)
                BeginLevel(currentLevelData);
        }
    }

    // Begins the level with the provided LevelData.
    public void BeginLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelManager.BeginLevel called with null LevelData.");
            return;
        }

        currentLevelData = levelData;
        StartCoroutine(LevelRoutine());
    }
    private IEnumerator LevelRoutine()
    {
        levelRunning = true;
        levelComplete = false;
        bossActive = false;

        // Ensure scrolling is active at start
        if (ScrollManager.Instance != null)
            ScrollManager.Instance.ResumeScrolling();

        // Use the enemy sequence defined in LevelData
        foreach (EnemySpawnInfo spawnInfo in currentLevelData.enemySequence)
        {
            // Delay before spawning this entry
            yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);

            SpawnEnemy(spawnInfo);

            // If this spawn is a boss, stop scrolling immediately
            if (spawnInfo.enemyData != null && spawnInfo.enemyData.type == EnemyType.Boss)
            {
                StartCoroutine(StopWhenBoss());
            }
        }

        // Wait until all active enemies have been removed
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        // Resume scrolling if boss was active
        if (bossActive && ScrollManager.Instance != null)
            ScrollManager.Instance.ResumeScrolling();

        levelRunning = false;
        levelComplete = true;

        ResultsScreen.SetActive(true);

        // Notify GameManager that the level completed
        if (GameManager.Instance != null)
            GameManager.Instance.OnLevelCompleted();
    }

    public IEnumerator StopWhenBoss()
    {
        if (ScrollManager.Instance != null)
            yield return new WaitForSeconds(2f);
            ScrollManager.Instance.StopScrolling();
        bossActive = true;
    }

    public void GoToHub()
    {
        GameManager.Instance.ReturnToHub();
    }


    // Spawns a single enemy based on spawn info. Uses pooling if available.
    private void SpawnEnemy(EnemySpawnInfo spawnInfo)
    {
        if (spawnInfo == null || spawnInfo.enemyData == null)
        {
            Debug.LogWarning("LevelManager.SpawnEnemy: spawnInfo or enemyData null.");
            return;
        }

        Vector3 spawnPos = enemySpawnPoint != null ? enemySpawnPoint.position : Vector3.zero;

        // Determine Y position based on enemy type and normalized enemyHeight
        if (spawnInfo.enemyData.type == EnemyType.Aerial)
        {
            float groundY = (groundReference != null) ? groundReference.position.y : 0f;
            float heightY = Mathf.Lerp(groundY, topOfScreenY, Mathf.Clamp01(spawnInfo.enemyHeight));
            spawnPos.y = heightY;
        }
        else
        {
            spawnPos.y = (groundReference != null) ? groundReference.position.y : 0f;
        }

        // Prefer pooling if available. Pools should be keyed by enemyID.
        GameObject enemyObj = null;
        if (ObjectPoolManager.Instance != null && !string.IsNullOrEmpty(spawnInfo.enemyData.enemyID))
        {
            enemyObj = ObjectPoolManager.Instance.SpawnFromPool(spawnInfo.enemyData.enemyID, spawnPos, Quaternion.identity);
        }
        else
        {
            // Use the prefab reference from EnemyData
            enemyObj = Instantiate(spawnInfo.enemyData.prefab, spawnPos, Quaternion.identity);
        }

        if (enemyObj != null)
        {
            activeEnemies.Add(enemyObj);

            // Attach and initialize EnemyBase if present
            EnemyBase enemyBase = enemyObj.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.OnEnemyDeath += HandleEnemyDeath;
                enemyBase.Initialize(spawnInfo.enemyData);
            }
            else
            {
                Debug.LogWarning("Spawned enemy prefab does not have an EnemyBase component.");
            }
        }
    }

    // Called when an enemy dies
    private void HandleEnemyDeath(GameObject enemyObj)
    {
        if (enemyObj == null) return;

        if (activeEnemies.Contains(enemyObj))
            activeEnemies.Remove(enemyObj);
    }

    // Force-clear active enemies (used on level reset/player death).
    public void ClearActiveEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject e = activeEnemies[i];
            if (e != null)
                e.SetActive(false);
        }

        activeEnemies.Clear();
    }

    public bool IsLevelComplete()
    {
        return levelComplete;
    }
}

