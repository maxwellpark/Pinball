using Events;
using System.Collections;
using UnityEngine;

public class Kicker : BounceBehaviourBase
{
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private SpriteRenderer kickerEdgeSprite;
    [SerializeField] private float colorFlashDuration = 0.2f;

    private Color originalColor;
    protected override bool IncludeGhostBalls => true;
    protected override bool OnCollisionOnlyOnce => false;
    protected override bool UseOnCollisionEnter => true;
    protected override bool UseOnTriggerEnter => false;

    private void Start()
    {
        if (kickerEdgeSprite != null)
        {
            originalColor = kickerEdgeSprite.color;
        }
    }

    protected override void OnCollision(Collider2D collider)
    {
        base.OnCollision(collider);

        if (kickerEdgeSprite != null)
        {
            StartCoroutine(FlashColor());
        }
    }

    protected override void OnBoardChanged(BoardChangedEvent evt)
    {
        if (audioSource != null && evt.Config.KickerSound)
        {
            collisionSound = evt.Config.KickerSound;
        }
    }

    private IEnumerator FlashColor()
    {
        kickerEdgeSprite.color = hitColor;
        yield return new WaitForSeconds(colorFlashDuration);
        kickerEdgeSprite.color = originalColor;
    }
}
