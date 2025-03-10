using UnityEngine;
using UnityEngine.Events;

public abstract class CollisionBehaviourBase : MonoBehaviour
{
    [SerializeField] private int score;

    public event UnityAction<int> OnScoreAdded;

    protected abstract void OnCollision(Collider2D collider);

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.IsBallOrGhostBall())
        {
            return;
        }

        Debug.Log($"{name} collided with {collision.gameObject.name}");

        if (score > 0)
        {
            GameManager.AddScore(score);
            OnScoreAdded?.Invoke(score);
        }

        OnCollision(collision.collider);
    }
}