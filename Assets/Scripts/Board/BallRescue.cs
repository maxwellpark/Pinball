using System.Collections;
using UnityEngine;

public class BallRescue : Buff
{
    [SerializeField] private Transform destination;
    [SerializeField] private float travelSpeed = 5f;

    private bool isSaving;

    protected override bool ShouldTrigger(Collider2D collision)
    {
        return base.ShouldTrigger(collision) && !isSaving;
    }

    protected override void TriggerBehaviour(Collider2D collision)
    {
        StartCoroutine(SaveBall(collision.gameObject));
    }

    private IEnumerator SaveBall(GameObject ball)
    {
        isSaving = true;
        var ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.velocity = Vector2.zero;
        ballRb.isKinematic = true;

        while (ball != null && Vector2.Distance(ball.transform.position, destination.position) > 0.05f)
        {
            ball.transform.position = Vector2.MoveTowards(ball.transform.position, destination.position, travelSpeed * Time.deltaTime);
            yield return null;
        }

        if (ball != null)
        {
            ball.transform.position = destination.position;
            ballRb.velocity = Vector2.zero;
            ballRb.isKinematic = false;
        }
        isSaving = false;
    }
}
