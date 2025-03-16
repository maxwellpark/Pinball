using UnityEngine;
using UnityEngine.Events;

public abstract class CollisionBehaviourBase : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private AudioClip collisionSound;

    // TODO: should these be configurable? 
    //[SerializeField] private bool useOnCollisionEnter = true;
    //[SerializeField] private bool useOnTriggerEnter;
    //[SerializeField] private bool includeGhostBalls = true;

    private AudioSource audioSource;
    protected abstract bool UseOnCollisionEnter { get; }
    protected abstract bool UseOnTriggerEnter { get; }
    protected abstract bool IncludeGhostBalls { get; }
    protected abstract void OnCollision(Collider2D collider);

    public event UnityAction<int> OnScoreAdded;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && collisionSound == null)
        {
            Debug.LogWarning($"{name} has an AudioSource but no collisionSound clip assigned");
        }
    }

    private void OnEnter(Collider2D collider)
    {
        if (IncludeGhostBalls ? !collider.IsBallOrGhostBall() : !collider.IsBall())
        {
            return;
        }

        Debug.Log($"{name} collided with {collider.gameObject.name}");

        if (score > 0)
        {
            GameManager.AddScore(score);
            OnScoreAdded?.Invoke(score);
        }

        // Sound is optional for now 
        if (audioSource != null && collisionSound != null)
        {
            audioSource.clip = collisionSound;
            audioSource.Stop();
            audioSource.Play();
        }

        OnCollision(collider);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (UseOnCollisionEnter)
        {
            OnEnter(collision.collider);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (UseOnTriggerEnter)
        {
            OnEnter(collider);
        }
    }
}
