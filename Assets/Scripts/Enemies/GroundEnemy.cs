using System.Collections;
using UnityEngine;

public class Enemy_Ground : EnemyBase
{
    public float knockbackForce = 20f;
    public float knockbackUpward = 10f;

    private float spawnY;
    public bool isKnockedBack = false;
    private Rigidbody2D rb;
    private float MinLeftPos = -1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnY = transform.position.y; // Record Y at spawn
    }

    private void Update()
    {
        MoveLeft();
        CheckTooFarLeft();
        CheckGrounded();
    }

    private void MoveLeft()
    {
        if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(-speed, 0f);
        }
    }

    private void CheckGrounded()
    {
        // Grounded if back to spawn Y (or below slightly)
        if (transform.position.y <= spawnY)
        {
            // Snap back to spawnY to prevent sinking
            Vector3 pos = transform.position;
            pos.y = spawnY;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isKnockedBack = true;
            ApplyKnockback();
        }
    }

    private void CheckTooFarLeft()
    {
        if (transform.position.x <= MinLeftPos)
        {
            Vector3 pos = transform.position;
            pos.x = MinLeftPos;
            transform.position = pos;
        }
    }

    public void ApplyKnockback()
    {
        isKnockedBack = true;
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;

        // Apply rightwards and slightly upwards impulse
        rb.linearVelocity = new Vector2(knockbackForce, knockbackUpward);
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = new Vector2(knockbackForce / 2, -knockbackUpward);
        // Wait until back to spawnY
        while (transform.position.y > spawnY + 0.01f)
            yield return null;

        isKnockedBack = false;
    }
}

