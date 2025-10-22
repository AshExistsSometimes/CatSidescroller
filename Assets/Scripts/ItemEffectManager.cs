using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private Dictionary<string, float> activeEffects = new Dictionary<string, float>();

    /// Activate an item effect for a duration (seconds)
    public void ActivateEffect(string itemID, float duration, float effectAmount)
    {
        if (activeEffects.ContainsKey(itemID))
            StopCoroutine(RemoveEffectAfter(itemID));

        activeEffects[itemID] = effectAmount;
        StartCoroutine(RemoveEffectAfter(itemID, duration));
    }

    public bool IsEffectActive(string itemID)
    {
        return activeEffects.ContainsKey(itemID);
    }

    public float GetEffectAmount(string itemID)
    {
        if (activeEffects.TryGetValue(itemID, out float amount))
            return amount;
        return 0f;
    }

    private System.Collections.IEnumerator RemoveEffectAfter(string itemID, float duration = 0f)
    {
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        if (activeEffects.ContainsKey(itemID))
            activeEffects.Remove(itemID);
    }
}

