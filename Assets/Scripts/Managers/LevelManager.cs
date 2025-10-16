using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Data Reference")]
    public LevelData currentLevelData;

    [Header("Spawn Settings")]
    public Transform enemySpawnPoint;  // Typically offscreen right
    public Transform groundReference;  // Used to define Y=0 position for grounded enemies
    public float topOfScreenY = 5f;    // Defines Y=1 equivalent for aerial enemies

    [Header("Spawned Enemies (Debug)")]
    public List<GameObject> activeEnemies = new List<GameObject>();

    private bool levelRunning = false;
    private bool bossActive = false;
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Starts the level with the specified LevelData.
    /// </summary>
    public void BeginLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        StartCoroutine(LevelRoutine());
    }

    /// <summary>
    /// Core coroutine that handles sequential enemy spawns and boss logic.
    /// </summary>
    private IEnumerator LevelRoutine()
    {
        levelRunning = true;
        levelComplete = false;
        bossActive = false;

        // Begin background scroll
        ScrollManager.Instance.ResumeScrolling();

        // Iterate through the enemy spawn list
        foreach (EnemySpawnInfo spawnInfo in currentLevelData.enemySpawnList)
        {
            yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);

            SpawnEnemy(spawnInfo);

            // If this enemy is a boss, stop scrolling
            if (spawnInfo.enemyData.type == EnemyType.Boss)
            {
                ScrollManager.Instance.StopScrolling();
                bossActive = true;
            }
        }

        // Wait until all enemies are cleared
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        // Resume scroll if boss was active (visual transition)
        if (bossActive)
            ScrollManager.Instance.ResumeScrolling();

        levelRunning = false;
        levelComplete = true;

        // Notify GameManager
        GameManager.Instance.OnLevelCompleted();
    }

    /// Spawns a single enemy from ObjectPool or via Instantiate if not pooled.
    private void SpawnEnemy(EnemySpawnInfo spawnInfo)
    {
        if (spawnInfo.enemyData == null)
        {
            Debug.LogWarning("LevelManager: Missing EnemyData in spawn info.");
            return;
        }

        Vector3 spawnPos = enemySpawnPoint.position;

        // Handle flying enemy height
        if (spawnInfo.enemyData.type == EnemyType.Aerial)
        {
            float heightY = Mathf.Lerp(groundReference.position.y, topOfScreenY, spawnInfo.enemyHeight);
            spawnPos.y = heightY;
        }
        else
        {
            spawnPos.y = groundReference.position.y;
        }

        GameObject enemyObj = ObjectPoolManager.Instance != null
            ? ObjectPoolManager.Instance.SpawnFromPool(spawnInfo.enemyData.enemyID, spawnPos, Quaternion.identity)
            : Instantiate(spawnInfo.enemyData.prefabRef, spawnPos, Quaternion.identity);

        if (enemyObj != null)
        {
            activeEnemies.Add(enemyObj);

            EnemyBase enemyBase = enemyObj.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.OnEnemyDeath += HandleEnemyDeath;
                enemyBase.Initialize(spawnInfo.enemyData);
            }
        }
    }

    // Handles cleanup when an enemy is defeated.
    private void HandleEnemyDeath(GameObject enemyObj)
    {
        if (activeEnemies.Contains(enemyObj))
            activeEnemies.Remove(enemyObj);
    }

    // Forces all active enemies to despawn (used on player death or reset).
    public void ClearActiveEnemies()
    {
        foreach (GameObject e in activeEnemies)
        {
            if (e != null)
                e.SetActive(false);
        }
        activeEnemies.Clear();
    }

    // Returns true if all enemies have been spawned and defeated.
    public bool IsLevelComplete()
    {
        return levelComplete;
    }
}

