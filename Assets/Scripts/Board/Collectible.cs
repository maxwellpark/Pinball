using UnityEngine;

public class Collectible : CollisionBehaviourBase
{
    [SerializeField] private GameObject particlePrefab;
    protected override bool IncludeGhostBalls => false;
    protected override bool UseOnCollisionEnter => false;
    protected override bool UseOnTriggerEnter => true;

    protected override void OnCollision(Collider2D collider)
    {
        // TODO: put particle spawning on the base class? 
        Instantiate(particlePrefab, collider.transform.position, particlePrefab.transform.rotation);
        Destroy(gameObject);
    }
}
