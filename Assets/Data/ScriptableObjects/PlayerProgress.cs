using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerProgress", menuName = "Data/Player Progress")]
public class PlayerProgress : ScriptableObject
{
    [Header("Leveling")]
    public int playerLevel = 1;
    public int playerExp = 0;
    public int nextLevelExp = 100;
    public int skillPoints = 0;

    [Header("Stats")]
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float baseDamage = 10f;

    [Header("Economy")]
    public int money = 0;

    [Header("Inventory")]
    public int teaCount = 0;
    public int milkCount = 0;
    public int elixirCount = 0;

    [Header("Cosmetics")]
    public List<string> cosmeticsOwned = new List<string>();

    // Converts this ScriptableObject data into a serializable save model.
    public PlayerProgressModel ToModel()
    {
        return new PlayerProgressModel
        {
            playerLevel = playerLevel,
            playerExp = playerExp,
            nextLevelExp = nextLevelExp,
            skillPoints = skillPoints,
            currentHP = currentHP,
            maxHP = maxHP,
            baseDamage = baseDamage,
            money = money,
            teaCount = teaCount,
            milkCount = milkCount,
            elixirCount = elixirCount,
            cosmeticsOwned = cosmeticsOwned.ToArray()
        };
    }

    // Loads data from model into this ScriptableObject instance.
    public void FromModel(PlayerProgressModel model)
    {
        playerLevel = model.playerLevel;
        playerExp = model.playerExp;
        nextLevelExp = model.nextLevelExp;
        skillPoints = model.skillPoints;
        currentHP = model.currentHP;
        maxHP = model.maxHP;
        baseDamage = model.baseDamage;
        money = model.money;
        teaCount = model.teaCount;
        milkCount = model.milkCount;
        elixirCount = model.elixirCount;
        cosmeticsOwned = new List<string>(model.cosmeticsOwned);
    }
}

// JSON-serializable player data container used by the save/load system.
[System.Serializable]
public class PlayerProgressModel
{
    public int playerLevel;
    public int playerExp;
    public int nextLevelExp;
    public int skillPoints;
    public float currentHP;
    public float maxHP;
    public float baseDamage;
    public int money;
    public int teaCount;
    public int milkCount;
    public int elixirCount;
    public string[] cosmeticsOwned;
}

