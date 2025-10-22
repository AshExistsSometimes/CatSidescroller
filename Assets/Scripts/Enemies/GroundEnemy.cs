using UnityEngine;

public class Enemy_Ground : EnemyBase
{
    public float speed = 2f;
    public float knockbackForce = 5f;
    public float knockbackUpward = 2f;

    private float spawnY;
    private bool isKnockedBack = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnY = transform.position.y; // Record Y at spawn
    }

    private void Update()
    {
        if (!isKnockedBack)
        {
            // Move left continuously
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }

        CheckGrounded();
    }

    private void CheckGrounded()
    {
        // Grounded if back to spawn Y (or below slightly)
        if (transform.position.y <= spawnY + 0.05f)
        {
            // Snap back to spawnY to prevent sinking
            Vector3 pos = transform.position;
            pos.y = spawnY;
            transform.position = pos;
        }
    }

    public void ApplyKnockback()
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine());
    }

    private System.Collections.IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;

        // Apply rightwards and slightly upwards impulse
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(1f, 0.5f).normalized * knockbackForce, ForceMode2D.Impulse);

        // Wait until back to spawnY
        while (transform.position.y > spawnY + 0.01f)
            yield return null;

        isKnockedBack = false;
    }
}

