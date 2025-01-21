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
        if (collider.TryGetComponent<Rigidbody2D>(out var rb))
        {
            var direction = (collider.transform.position - transform.position).normalized;
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}