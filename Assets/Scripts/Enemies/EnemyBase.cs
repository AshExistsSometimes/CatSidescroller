using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour
{
    public event Action<GameObject> OnEnemyDeath;

    [Header("Stats")]
    public float maxHP = 1f;
    public float currentHP = 1f;
    public float damage = 1f;
    public float speed = 1f;
    public EnemyType enemyType;

    protected EnemyData sourceData;

    // Cached components
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    // Hit feedback
    private Color originalColor;
    private bool isKnockedBack = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    /// <summary>
    /// Initializes enemy stats from ScriptableObject data.
    /// </summary>
    public virtual void Initialize(EnemyData data)
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

        gameObject.name = $"Enemy_{data.enemyID}";
    }

    /// Called when the enemy takes damage. Triggers flash and death if HP <= 0.
    public virtual void TakeDamage(float amount)
    {
        Debug.Log("Enemy Damaged");
        if (amount <= 0f) return;
        currentHP -= amount;

        StartCoroutine(FlashRed());

        if (currentHP <= 0f)
            Die();
    }

    /// Coroutine that briefly flashes the sprite red to indicate damage.
    protected IEnumerator FlashRed()
    {
        Debug.Log("Flashing Red");
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    /// Applies knockback force, preventing movement temporarily.
    public virtual void ApplyKnockback(Vector2 direction, float force, float duration = 0.3f)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }

    protected virtual void Die()
    {
        OnEnemyDeath?.Invoke(gameObject);
        EventBus.Publish(GameEvent.EnemyDefeated, sourceData);

        if (ObjectPoolManager.Instance != null && sourceData != null && !string.IsNullOrEmpty(sourceData.enemyID))
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}


