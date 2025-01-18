using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleBumper : Bumper
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float minDamage = 30f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private int scoreOnDestroy = 250;
    [SerializeField] private float damageInterval = 1f;

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Color fullHealthColor = Color.white;
    [SerializeField] private Color damagedColor = Color.red;

    private float currentHealth;
    private float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            UpdateColor();
        }
    }

    private SpriteRenderer spriteRenderer;

    public event UnityAction<DestructibleBumper> OnDestroyed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    // Too lazy to change OnCollision signature for now, so stick stuff in here to get relativeVelocity.
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (!Utils.IsBallOrGhostBall(collision))
        {
            return;
        }

        var impactForce = collision.relativeVelocity.magnitude;
        var damage = Mathf.Max(impactForce * damageMultiplier, minDamage);
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        Debug.Log($"[d. bumper] {name} taking {damage} damage (was {CurrentHealth} health)...");
        CurrentHealth -= damage;
        Debug.Log($"[d. bumper] {name} new health: {CurrentHealth}");

        if (CurrentHealth <= 0f)
        {
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            GameManager.AddScore(scoreOnDestroy);
            OnDestroyed?.Invoke(this);
            Destroy(gameObject, 0.25f);
        }
    }

    private void UpdateColor()
    {
        var healthPercentage = CurrentHealth / maxHealth;
        spriteRenderer.color = Color.Lerp(damagedColor, fullHealthColor, healthPercentage);
    }

    public void StartDamageOverTime(float damage, float duration)
    {
        StartCoroutine(DamageOverTime(damage, duration));
    }

    private IEnumerator DamageOverTime(float damage, float duration)
    {
        var time = 0f;

        while (time < duration && CurrentHealth > 0)
        {
            yield return new WaitForSeconds(damageInterval);
            TakeDamage(damage / damageInterval);
            time += damageInterval;
        }
    }
}
