using UnityEngine;

public abstract class BounceBehaviourBase : CollisionBehaviourBase
{
    [SerializeField] protected float force = 10f;

    protected virtual void ApplyForce(Collider2D collider)
    {
        if (collider.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            var direction = (collider.transform.position - transform.position).normalized;
            ballRb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        Debug.Log($"{name} collided with {collision.gameObject.name}");
        ApplyForce(collision.collider);
        OnCollision(collision.collider);
    }
}