using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AerialEnemy : EnemyBase
{
    private Transform player;
    public float verticalFollowSpeed = 2f;
    public float wobbleAmplitude = 0.3f;
    public float wobbleFrequency = 3f;
    public float groundHeight = 0.1f;
    public bool isKnockedBack = false;
    public float knockbackForce = 5f;
    public float knockbackUpward = 4f;

    private void Start()
    {
        if (GameObject.FindWithTag("Player") != null)
            player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;
        if (rb == null) return;

        if (currentHP <= 0f) return;

        if (!isKnockedBack)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, player.position.x, speed * Time.deltaTime);

            // Smooth Y-following with wobble
            float targetY = Mathf.Lerp(pos.y, player.position.y, Time.deltaTime * verticalFollowSpeed);
            float wobble = Mathf.Sin(Time.time * wobbleFrequency) * (wobbleAmplitude / 100);
            pos.y = targetY + wobble;

            transform.position = pos;
        }

        CheckGrounded();
    }

    private void CheckGrounded()
    {
        // Grounded if back to spawn Y (or below slightly)
        if (transform.position.y <= groundHeight)
        {
            // Snap back to ground Y to prevent sinking
            Vector3 pos = transform.position;
            pos.y = groundHeight;
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

        isKnockedBack = false;
    }
}
