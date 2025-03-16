using UnityEngine;

public class Collectible : CollisionBehaviourBase
{
    [SerializeField] private GameObject particlePrefab;
    protected override bool IncludeGhostBalls => false;
    protected override bool OnCollisionOnlyOnce => true;
    protected override bool UseOnCollisionEnter => false;
    protected override bool UseOnTriggerEnter => true;

    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnCollision(Collider2D collider)
    {
        // TODO: put particle spawning on the base class? 
        Instantiate(particlePrefab, collider.transform.position, particlePrefab.transform.rotation);

        // Hack to make sure we keep playing the audio source after object is destroyed 
        // TODO: we could probably move the audio source onto the player 
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1.5f);
    }
}
