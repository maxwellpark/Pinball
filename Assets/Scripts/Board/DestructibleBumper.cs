using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleBumper : Bumper
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private int scoreOnDestroy = 250;
    [SerializeField] private float damageInterval = 1f;

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Color fullHealthColor = Color.white;
    [SerializeField] private Color damagedColor = Color.red;

    private SpriteRenderer spriteRenderer;

    public event UnityAction<DestructibleBumper> OnDestroyed;

    protected override void Start()
    {
        base.Start();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = fullHealthColor;
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
        var damage = impactForce * damageMultiplier;
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        Debug.Log($"[d. bumper] {name} taking {damage} damage (was {currentHealth} health)...");
        currentHealth -= damage;
        Debug.Log($"[d. bumper] {name} new health: {currentHealth}");
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateColor();

        if (currentHealth <= 0f)
        {
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            GameManager.AddScore(scoreOnDestroy);
            OnDestroyed?.Invoke(this);
            Destroy(gameObject, 0.5f);
        }
    }

    private void UpdateColor()
    {
        var healthPercentage = currentHealth / maxHealth;
        spriteRenderer.color = Color.Lerp(damagedColor, fullHealthColor, healthPercentage);
    }

    public void StartDamageOverTime(float damage, float duration)
    {
        StartCoroutine(DamageOverTime(damage, duration));
    }

    private IEnumerator DamageOverTime(float damage, float duration)
    {
        var time = 0f;

        while (time < duration && currentHealth > 0)
        {
            yield return new WaitForSeconds(damageInterval);
            TakeDamage(damage / damageInterval);
            time += damageInterval;
        }
    }
}
