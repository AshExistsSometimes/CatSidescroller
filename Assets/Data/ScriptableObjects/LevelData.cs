using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Identification")]
    public string levelID;
    public string sceneName;
    public ZoneData zoneData;

    [Header("Rewards")]
    public int baseRewardMoney = 10;
    public int minChestMoney = 5;
    public int maxChestMoney = 20;

    [Header("Enemies")]
    public List<EnemyData> enemySpawnList = new List<EnemyData>();
    public bool hasBoss = false;
    public EnemyData bossEnemyData;

    [Header("Level Bounds")]
    public Transform startPoint;
    public Transform endPoint;
}
