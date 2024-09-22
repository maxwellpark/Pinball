using System.Collections;
using UnityEngine;

public class Kicker : BounceBehaviourBase
{
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private SpriteRenderer kickerEdgeSprite;
    [SerializeField] private float colorFlashDuration = 0.2f;

    private Color originalColor;

    private void Start()
    {
        if (kickerEdgeSprite != null)
        {
            originalColor = kickerEdgeSprite.color;
        }
    }

    protected override void OnCollision(Collider2D collider)
    {
        if (kickerEdgeSprite != null)
        {
            StartCoroutine(FlashColor());
        }
    }

    private IEnumerator FlashColor()
    {
        kickerEdgeSprite.color = hitColor;
        yield return new WaitForSeconds(colorFlashDuration);
        kickerEdgeSprite.color = originalColor;
    }
}
