using UnityEngine;

public class Attractor : MonoBehaviour
{
    [SerializeField] private float pullStrength = 10f;
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float gravityOverride;

    private void FixedUpdate()
    {
        var ballRb = GameManager.BallRb;
        if (ballRb == null)
        {
            return;
        }

        // Way of stopping default gravity interfering with pull force 
        var originalGravity = ballRb.gravityScale;
        ballRb.gravityScale = gravityOverride;

        var position = (Vector2)transform.position;
        var direction = (position - ballRb.position).normalized;
        var distance = Vector2.Distance(position, ballRb.position);

        if (distance < stopDistance)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.gravityScale = originalGravity;
            return;
        }

        // Increase force inversely to distance 
        var pullForce = pullStrength / Mathf.Pow(distance, 2);
        pullForce = Mathf.Min(pullForce, pullStrength);
        Debug.Log($"[attractor] pull force = {pullForce} | distance = {distance}");

        ballRb.AddForce(direction * pullForce, ForceMode2D.Force);
        ballRb.gravityScale = originalGravity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
