using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackInput : MonoBehaviour
{
    [Header("Swipe Settings")]
    public Camera mainCamera;
    public GameObject swipeTrailPrefab;
    public float swipeTrailLifetime = 0.3f;

    private Vector2 swipeStart;
    private bool isSwiping = false;
    private GameObject currentTrail;

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

            // Tap damage (click without moving)
            ApplyTapDamage(swipeStart);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isSwiping)
            {
                Vector2 swipeEnd = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                ApplySwipeDamage(swipeStart, swipeEnd);
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

                ApplyTapDamage(swipeStart);
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                UpdateSwipeTrail(touchPos);
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isSwiping)
            {
                ApplySwipeDamage(swipeStart, touchPos);
                Destroy(currentTrail, swipeTrailLifetime);
                isSwiping = false;
            }
        }
    }
    #endregion

    private void CreateSwipeTrail(Vector2 startPos)
    {
        if (swipeTrailPrefab != null)
        {
            currentTrail = Instantiate(swipeTrailPrefab, startPos, Quaternion.identity);
        }
    }

    private void UpdateSwipeTrail(Vector2 currentPos)
    {
        if (currentTrail != null)
        {
            currentTrail.transform.position = currentPos;
            Vector2 dir = currentPos - swipeStart;
            if (dir.sqrMagnitude > 0.001f)
                currentTrail.transform.right = dir.normalized;
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

    private void DealDamage(EnemyBase enemy)
    {
        float damage = GameManager.Instance.playerProgress.baseDamage;

        // Strength Elixir check
        if (ItemEffectManager.Instance != null && ItemEffectManager.Instance.IsEffectActive("Item_Elixir"))
            damage *= 1f + (ItemEffectManager.Instance.GetEffectAmount("Item_Elixir") / 100f);

        enemy.TakeDamage(damage);
    }
}
