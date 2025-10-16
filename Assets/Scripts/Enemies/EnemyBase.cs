using System;
using UnityEngine;

/// <summary>
/// Minimal base class for enemies. Holds stats, provides Initialize and damage handling,
/// and fires OnEnemyDeath when the enemy dies. Expand this for specific behaviours.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    // Public event that notifies subscribers that this enemy has died.
    // The GameObject parameter is the enemy instance (useful for pooling).
    public event Action<GameObject> OnEnemyDeath;

    // Runtime stats populated from EnemyData
    public float maxHP = 1f;
    public float currentHP = 1f;
    public float damage = 1f;
    public float speed = 1f;
    public EnemyType enemyType;

    // Reference to the EnemyData used to initialize this instance
    private EnemyData sourceData;

    /// <summary>
    /// Initialize this enemy instance using data from EnemyData.
    /// Should be called immediately after instantiation or pooling.
    /// </summary>
    public void Initialize(EnemyData data)
    {
        if (data == null)
        {
            Debug.LogWarning("EnemyBase.Initialize called with null data.");
            return;
        }

        sourceData = data;
        enemyType = data.type;
        maxHP = data.maxHP;
        currentHP = maxHP;
        damage = data.damage;
        speed = data.speed;

        // Optional: set name for debugging
        gameObject.name = $"Enemy_{data.enemyID}";
    }

    /// <summary>
    /// Apply damage to the enemy. When HP <= 0 this will trigger death logic.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHP -= amount;
        if (currentHP <= 0f)
            Die();
        else
            OnHitFeedback();
    }

    /// <summary>
    /// Hook for hit feedback (particles, flash, sound). Keep minimal here.
    /// </summary>
    protected virtual void OnHitFeedback()
    {
        // Placeholder: override in derived classes or add particle playback.
    }

    /// <summary>
    /// Handles death: disable or return to pool and notify listeners.
    /// </summary>
    protected virtual void Die()
    {
        // Notify listeners before deactivating
        OnEnemyDeath?.Invoke(gameObject);

        // Broadcast global event (optional)
        EventBus.Publish(GameEvent.EnemyDefeated, sourceData);

        // If pooling is used, deactivate; otherwise destroy.
        if (ObjectPoolManager.Instance != null && sourceData != null && !string.IsNullOrEmpty(sourceData.enemyID))
        {
            // Return to pool indirectly by disabling; pooled object should be recycled by pool rules.
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

