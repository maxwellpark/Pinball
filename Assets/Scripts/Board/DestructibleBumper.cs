using UnityEngine;

public class DestructibleBumper : Bumper
{
    [SerializeField] private int health = 3;
    [SerializeField] private int score = 250;

    private SpriteRenderer spriteRenderer;
    private Color[] damageColors;

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
        health--;
        UpdateColor();

        if (health <= 0)
        {
            GameManager.AddScore(score);
            Destroy(gameObject, 0.5f);
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
