using UnityEngine;

public enum ItemEffectType
{
    DamageReduction,
    Heal,
    DamageIncrease
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identification")]
    public string itemID;
    public string itemName;

    [Header("Effect")]
    public ItemEffectType effectType;
    public float effectAmount = 0.5f;    // Multiplier (e.g., 0.5 = 50% reduction or increase)
    public float duration = 5f;          // Duration in seconds for buff effects
    public float cooldown = 10f;         // Cooldown before reuse

    [Header("Economy")]
    public int cost = 50;
}

