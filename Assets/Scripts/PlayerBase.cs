using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerBase : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHP = 10f;
    public float currentHP = 10f;
    public bool isAlive = true;
    public float iFrameDuration = 0.5f;

    private bool invulnerable = false;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        currentHP = maxHP;
        isAlive = true;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive || invulnerable) return;

        currentHP -= amount;
        StartCoroutine(FlashIFrames());

        if (currentHP <= 0) Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage);

            // Knock enemy to the right with slight upward force
            Vector2 knockDir = new Vector2(1f, 0.5f);
            float knockForce = 5f;
            enemy.ApplyKnockback(knockDir, knockForce, 0.3f);
        }
    }

    private IEnumerator FlashIFrames()
    {
        invulnerable = true;
        float flashTime = 0.1f;
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashTime);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashTime);
        }
        invulnerable = false;
    }

    private void Die()
    {
        isAlive = false;
        currentHP = 0;
        Debug.Log("Player has died.");
        EventBus.Publish(GameEvent.PlayerDied);
        if (LevelManager.Instance != null)
            LevelManager.Instance.ClearActiveEnemies();
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToHub();
    }
}
