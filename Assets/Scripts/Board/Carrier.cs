using System.Collections;
using UnityEngine;

public class Carrier : MonoBehaviour
{
    [SerializeField] private Transform slot;
    [SerializeField] private Transform destination;
    [SerializeField] private float durationInSeconds = 2f;

    private Vector3 startPos;
    private bool isCarrying;

    private void Start()
    {
        startPos = slot.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision) || isCarrying)
        {
            return;
        }

        var transform = collision.transform;
        var ballRb = transform.GetComponent<Rigidbody2D>();

        transform.position = this.transform.position;

        if (ballRb != null)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.simulated = false;

            var trail = ballRb.GetComponent<TrailRenderer>();
            trail.Clear();
        }

        StartCoroutine(Carry(transform));
    }

    private IEnumerator Carry(Transform ballTrans)
    {
        isCarrying = true;

        var time = 0f;
        while (time < durationInSeconds)
        {
            time += Time.deltaTime;
            slot.position = Vector3.Lerp(startPos, destination.position, time / durationInSeconds);
            ballTrans.position = slot.position;
            yield return null;
        }

        slot.position = startPos;
        ballTrans.position = destination.position;

        if (ballTrans.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            ballRb.simulated = true;
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        isCarrying = false;
    }
}
