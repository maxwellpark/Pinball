using System.Collections;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField] private Transform entrance;
    [SerializeField] private Transform exit;
    [SerializeField] private float durationInSeconds = 1f;
    [SerializeField] private float force = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsBall())
        {
            return;
        }

        var ballTrans = collision.transform;

        if (ballTrans.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        StartCoroutine(Move(ballTrans, ballRb));
    }

    private IEnumerator Move(Transform transform, Rigidbody2D rb)
    {
        rb.isKinematic = true;
        transform.position = entrance.position;
        rb.isKinematic = false;

        var time = 0f;
        var direction = (exit.position - entrance.position).normalized;

        while (time < durationInSeconds)
        {
            rb.velocity = direction * force;
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = exit.position;
        rb.velocity = Vector2.zero;
    }
}
