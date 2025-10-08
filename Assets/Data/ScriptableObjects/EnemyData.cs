using UnityEngine;

public enum EnemyType { Ground, Aerial, Boss }

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identification")]
    public string enemyID;
    public EnemyType type;

    [Header("Stats")]
    public float maxHP = 1f;
    public float damage = 1f;
    public float speed = 1f;

    [Header("Aerial Movement (Only used if type is aerial)")]
    [Range(0f, 180f)] public float movDeviation = 0f;
    public float deviationRate = 0f;

    [Header("Rewards")]
    public int rewardXP = 10;
    public int rewardMoney = 5;

    [Header("References")]
    public GameObject prefab;
}
