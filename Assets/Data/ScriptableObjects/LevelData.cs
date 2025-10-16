using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemySpawnInfo
{
    [Tooltip("Enemy to spawn at this point in the level.")]
    public EnemyData enemyData;

    [Tooltip("Delay in seconds after the previous enemy spawns.")]
    public float delayBeforeSpawn = 1.0f;

    [Tooltip("Vertical height offset for aerial enemies. Ignored for ground enemies.")]
    [Range(0f, 1f)]
    public float enemyHeight = 0f;
}

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("Unique ID used internally to identify this level.")]
    public string levelID;

    [Tooltip("Name of the Unity scene this level belongs to (usually shared for all levels).")]
    public string sceneName;

    [Tooltip("Zone definition for visual and environmental data.")]
    public ZoneData zoneData;

    [Header("Rewards")]
    [Tooltip("Base money awarded upon level completion.")]
    public int baseRewardMoney = 10;

    [Tooltip("Minimum random money value that can be found in a chest during this level.")]
    public int minChestMoney = 5;

    [Tooltip("Maximum random money value that can be found in a chest during this level.")]
    public int maxChestMoney = 20;

    [Header("Enemy Sequence")]
    [Tooltip("Ordered list of enemies that spawn during the level.")]
    public List<EnemySpawnInfo> enemySequence = new List<EnemySpawnInfo>();

    [Header("Boss Settings")]
    [Tooltip("True if this level ends with a boss fight.")]
    public bool hasBoss = false;

    [Tooltip("Reference to the boss enemy data for this level (if hasBoss is true).")]
    public EnemyData bossEnemyData;

    [Header("Environment Overrides (Optional)")]
    [Tooltip("Additional layers added on top of the zone visuals.")]
    public List<LayerData> additionalLayers = new List<LayerData>();

    [Tooltip("Layers that replace zone visuals of the same type.")]
    public List<LayerData> overrideLayers = new List<LayerData>();
}