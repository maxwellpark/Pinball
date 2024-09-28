using UnityEngine;

public abstract class CollisionBehaviourBase : MonoBehaviour
{
    [SerializeField] private int score;

    protected abstract void OnCollision(Collider2D collider);

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision))
        {
            return;
        }

        Debug.Log($"{name} collided with {collision.gameObject.name}");

        if (score > 0)
        {
            GameManager.AddScore(score);
        }

        OnCollision(collision.collider);
    }
}