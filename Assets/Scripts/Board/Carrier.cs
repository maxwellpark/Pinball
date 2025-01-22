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
        if (!collision.IsBallOrGhostBall() || isCarrying)
        {
            return;
        }

        var transform = collision.transform;
        transform.position = this.transform.position;

        if (collision.TryGetComponent<Ball>(out var ball))
        {
            ball.Freeze();
        }

        StartCoroutine(Carry(transform));
    }

    private IEnumerator Carry(Transform ballTrans)
    {
        isCarrying = true;

        var time = 0f;
        while (time < durationInSeconds)
        {
            // Shouldn't be needed but for testing in case resetting balls while being carried 
            if (ballTrans == null)
            {
                isCarrying = false;
                yield break;
            }

            time += Time.deltaTime;
            slot.position = Vector3.Lerp(startPos, destination.position, time / durationInSeconds);
            ballTrans.position = slot.position;
            yield return null;
        }

        slot.position = startPos;
        ballTrans.position = destination.position;

        if (ballTrans.TryGetComponent<Ball>(out var ball))
        {
            ball.Unfreeze();
        }

        isCarrying = false;
    }
}
