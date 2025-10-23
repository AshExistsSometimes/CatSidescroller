using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class PlayerBase : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHP = 10f;
    public float currentHP = 10f;
    public bool isAlive = true;
    public float iFrameDuration = 0.75f;

    private bool invulnerable = false;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("UI")]
    public Slider HPSlider;
    public Image SliderFill;

    private void Start()
    {
        maxHP = GameManager.Instance.playerProgress.maxHP;

        currentHP = maxHP;
        isAlive = true;

        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive || invulnerable) return;

        currentHP -= amount;
        HPSlider.value = currentHP;
        UpdateSliderColour();
        StartCoroutine(FlashIFrames());

        if (currentHP <= 0) Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (!isAlive) return;
        Debug.Log(other.gameObject);

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage);
        }
    }

    private IEnumerator FlashIFrames()
    {
        invulnerable = true;
        float flashTime = 0.1f;
            sr.color = Color.red;
            yield return new WaitForSeconds(flashTime);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashTime);
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

    public void UpdateSliderColour()
    {
        if (currentHP >= (maxHP / 2))
        {
            SliderFill.color = Color.green;
        }

        else if (currentHP <= (maxHP / 2) && currentHP > (maxHP / 10))
        {
            SliderFill.color = Color.yellow;
        }

        else
        {
            SliderFill.color = Color.red;
        }
    }
}
