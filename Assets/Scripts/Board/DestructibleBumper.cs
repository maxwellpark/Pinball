using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructibleBumper : Bumper
{
    [SerializeField] private int health = 3;
    [SerializeField] private int scoreOnDestroy = 250;
    [SerializeField] private float damageInterval = 1f;

    private SpriteRenderer spriteRenderer;
    private Color[] damageColors;
    public event UnityAction<DestructibleBumper> OnDestroyed;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageColors = new[]
        {
            new Color(1f, 0.5f,0f),
            new Color(1f, 0.75f,0f),
            new Color(1f, 0.25f,0f),
        };

        UpdateColor();
    }

    protected override void OnCollision(Collider2D collider)
    {
        base.OnCollision(collider);
        TakeDamage();
    }

    private void TakeDamage()
    {
        health--;
        UpdateColor();

        if (health <= 0)
        {
            GameManager.AddScore(scoreOnDestroy);
            OnDestroyed?.Invoke(this);
            Destroy(gameObject, 0.5f);
        }
    }

    public void StartDamageOverTime(float duration)
    {
        StartCoroutine(DamageOverTime(duration));
    }

    private IEnumerator DamageOverTime(float duration)
    {
        var time = 0f;

        while (time < duration && health > 0)
        {
            yield return new WaitForSeconds(damageInterval);
            TakeDamage();
            time += damageInterval;
        }
    }

    private void UpdateColor()
    {
        if (health > 0 && health <= damageColors.Length)
        {
            spriteRenderer.color = damageColors[health - 1];
        }
        else if (health <= 0)
        {
            spriteRenderer.color = Color.clear;
        }
    }
}
