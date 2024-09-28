using UnityEngine;

public abstract class BounceBehaviourBase : CollisionBehaviourBase
{
    [SerializeField] protected float force = 10f;

    protected override void OnCollision(Collider2D collider)
    {
        ApplyForce(collider);
    }

    protected virtual void ApplyForce(Collider2D collider)
    {
        if (collider.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            var direction = (collider.transform.position - transform.position).normalized;
            ballRb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}