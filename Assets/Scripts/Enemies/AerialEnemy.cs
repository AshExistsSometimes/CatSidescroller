using UnityEngine;

public class AerialEnemy : EnemyBase
{
    private Transform player;
    public float verticalFollowSpeed = 2f;
    public float wobbleAmplitude = 0.3f;
    public float wobbleFrequency = 3f;

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

        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, player.position.x, speed * Time.deltaTime);

        // Smooth Y-following with wobble
        float targetY = Mathf.Lerp(pos.y, player.position.y, Time.deltaTime * verticalFollowSpeed);
        float wobble = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;
        pos.y = targetY + wobble;

        transform.position = pos;
    }
}
