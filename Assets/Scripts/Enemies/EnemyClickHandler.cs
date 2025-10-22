using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EnemyBase))]
public class EnemyClickHandler : MonoBehaviour, IPointerClickHandler
{
    private EnemyBase enemyBase;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
        if (enemyBase == null)
            Debug.LogWarning("EnemyClickHandler: EnemyBase not found on this object.");
    }

    /// Called when the enemy is clicked/tapped
    public void OnPointerClick(PointerEventData eventData)
    {
        if (enemyBase == null) return;

        float damage = 0f;

        // Get base damage from player progress
        if (GameManager.Instance != null && GameManager.Instance.playerProgress != null)
            damage = GameManager.Instance.playerProgress.baseDamage;

        // Check for active Strength Elixir effect
        if (ItemEffectManager.Instance != null && ItemEffectManager.Instance.IsEffectActive("Item_Elixir"))
        {
            float multiplier = ItemEffectManager.Instance.GetEffectAmount("Item_Elixir"); // e.g., 50
            damage *= 1f + (multiplier / 100f);
        }

        // Apply damage to enemy
        enemyBase.TakeDamage(damage);
    }
}

