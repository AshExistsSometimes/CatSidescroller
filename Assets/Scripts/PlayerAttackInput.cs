using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackInput : MonoBehaviour
{
    [Header("Swipe Settings")]
    public Camera mainCamera;
    public GameObject swipeTrailPrefab;
    public float swipeTrailLifetime = 0.3f;

    [Header("Tap Settings")]
    public GameObject tapHitmarkerPrefab;
    public float tapHitmarkerLifetime = 0.2f;
    public float swipeThreshold = 0.3f; // distance threshold to classify swipe vs tap

    private Vector2 swipeStart;
    private bool isSwiping = false;
    private GameObject currentTrail;

    public float spawnInterval = 0.001f;
    private float timeSinceLastSpawn = 0f;
    private Vector2 lastSpawnPos;

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    #region Mouse Input
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            swipeStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            CreateSwipeTrail(swipeStart);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isSwiping)
            {
                Vector2 swipeEnd = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                float swipeDistance = Vector2.Distance(swipeEnd, swipeStart);

                if (swipeDistance < swipeThreshold)
                {
                    ApplyTapDamage(swipeStart);
                    ShowTapHitmarker(swipeStart);
                }
                else
                {
                    ApplySwipeDamage(swipeStart, swipeEnd);
                }

                Destroy(currentTrail, swipeTrailLifetime);
                isSwiping = false;
            }
        }
        else if (isSwiping)
        {
            UpdateSwipeTrail(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    #endregion

    #region Touch Input
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = mainCamera.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                isSwiping = true;
                swipeStart = touchPos;
                CreateSwipeTrail(swipeStart);
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                UpdateSwipeTrail(touchPos);
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isSwiping)
            {
                float swipeDistance = Vector2.Distance(touchPos, swipeStart);

                if (swipeDistance < swipeThreshold)
                {
                    ApplyTapDamage(swipeStart);
                    ShowTapHitmarker(swipeStart);
                }
                else
                {
                    ApplySwipeDamage(swipeStart, touchPos);
                }

                Destroy(currentTrail, swipeTrailLifetime);
                isSwiping = false;
            }
        }
    }
    #endregion

    private void CreateSwipeTrail(Vector2 startPos)
    {
        lastSpawnPos = startPos;
        timeSinceLastSpawn = 0f;
    }

    private void UpdateSwipeTrail(Vector2 currentPos)
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            Vector2 dir = currentPos - lastSpawnPos;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);

                GameObject seg = Instantiate(swipeTrailPrefab, lastSpawnPos, rot);
                Destroy(seg, swipeTrailLifetime);

                lastSpawnPos = currentPos;
                timeSinceLastSpawn = 0f;
            }
        }
    }

    private void ApplySwipeDamage(Vector2 start, Vector2 end)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(start, 0.2f, end - start, Vector2.Distance(end, start));
        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
                DealDamage(enemy);
        }
    }

    private void ApplyTapDamage(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (var hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
                DealDamage(enemy);
        }
    }

    private void ShowTapHitmarker(Vector2 pos)
    {
        if (tapHitmarkerPrefab == null) return;

        float randomZ = Random.Range(-50f, 50f);
        Quaternion rot = Quaternion.Euler(0f, 0f, randomZ);

        GameObject marker = Instantiate(tapHitmarkerPrefab, pos, rot);
        Destroy(marker, tapHitmarkerLifetime);
    }

    private void DealDamage(EnemyBase enemy)
    {
        float damage = GameManager.Instance.playerProgress.baseDamage;

        if (ItemEffectManager.Instance != null && ItemEffectManager.Instance.IsEffectActive("Item_Elixir"))
            damage *= 1f + (ItemEffectManager.Instance.GetEffectAmount("Item_Elixir") / 100f);

        enemy.TakeDamage(damage);
    }
}
