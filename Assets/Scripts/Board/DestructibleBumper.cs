using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleBumper : Bumper
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool regensHealth;
    [SerializeField] private float regenRate = 5f;
    [SerializeField, Tooltip("Time after which the regen starts")] private float regenDelayInSeconds = 2f;

    [Header("Damage")]
    [SerializeField] private float minDamage = 30f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField, Tooltip("Damage over time tick speed")] private float damageInterval = 1f;
    [SerializeField] private int scoreOnDestroy = 250;

    [Header("Visuals")]
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
    private Coroutine regenCoroutine;
    private float lastDamageTime;

    public event UnityAction<DestructibleBumper> OnDestroyed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    private void Update()
    {
        if (regensHealth && regenCoroutine == null && CurrentHealth < maxHealth && Time.time >= lastDamageTime + regenDelayInSeconds)
        {
            Debug.Log("[d. bumper] starting regen at time " + Time.time);
            regenCoroutine = StartCoroutine(RegenHealth());
        }
    }

    private IEnumerator RegenHealth()
    {
        while (CurrentHealth < maxHealth)
        {
            CurrentHealth += regenRate * Time.deltaTime;
            yield return null;
        }

        Debug.Log("[d. bumper] ending regen at time " + Time.time);
        regenCoroutine = null;
    }

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

        // Cancel regen if active
        if (regenCoroutine != null)
        {
            Debug.Log("[d. bumper] cancelling regen at time " + Time.time);
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        lastDamageTime = Time.time;

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
