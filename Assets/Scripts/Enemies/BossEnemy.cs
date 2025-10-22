using UnityEngine;

public class BossEnemy : EnemyBase
{
    private void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.Translate(Vector3.left * (speed * 0.25f) * Time.deltaTime);

        if (transform.position.x < -30f)
            Die();
    }
}
