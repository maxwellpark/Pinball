using System.Collections;
using UnityEngine;

public abstract class ReceiverBase : MonoBehaviour
{
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private string animationName;

    protected virtual void OnEnter(Collider2D collision)
    {
        var transform = collision.transform;
        var ballRb = transform.GetComponent<Rigidbody2D>();

        transform.position = this.transform.position;

        if (ballRb != null)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.isKinematic = true;

            var trail = ballRb.GetComponent<TrailRenderer>();
            trail.Clear();
        }

        if (animationName != null)
        {
            var animator = collision.GetComponentInChildren<Animator>(true);
            animator.gameObject.SetActive(true);
            animator.Play(animationName);
        }
    }

    protected virtual void OnExit(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            ballRb.isKinematic = false;
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        if (animationName != null)
        {
            var animator = collision.GetComponentInChildren<Animator>();
            animator.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        StartCoroutine(Receive(collision));
    }

    private IEnumerator Receive(Collider2D collision)
    {
        OnEnter(collision);
        yield return new WaitForSeconds(waitTime);
        OnExit(collision);
    }
}
