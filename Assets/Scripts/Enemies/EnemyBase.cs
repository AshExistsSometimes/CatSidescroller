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
    public Color OriginalColour = Color.white;
    public Color DamageColour = Color.red;

    protected EnemyData sourceData;

    // Cached components
    protected Rigidbody2D rb;
    public SpriteRenderer sr;

    // Hit feedback
    private Color originalColor;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (amount <= 0f) return;
        currentHP -= amount;

        StartCoroutine(FlashRed());

        if (currentHP <= 0f)
            Die();
    }

    /// Coroutine that briefly flashes the sprite red to indicate damage.
    protected IEnumerator FlashRed()
    {
        Debug.Log("FLASH RED");
        sr.color = DamageColour;
        yield return new WaitForSeconds(0.1f);
        sr.color = OriginalColour;
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


