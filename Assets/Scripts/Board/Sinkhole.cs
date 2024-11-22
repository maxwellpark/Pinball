using UnityEngine;

public class Sinkhole : MonoBehaviour
{
    public float pullRadius = 5f;
    public float pullStrength = 10f;
    public float spiralSpeed = 180f;
    public float stopThreshold = 0.1f;
    public float minVelocityToEscape = 10f;
    public float pullSpeedFactor = 0.5f;

    private void FixedUpdate()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (var collider in colliders)
        {
            var rb = collider.attachedRigidbody;

            if (rb != null && Utils.IsBall(collider))
            {
                if (rb.velocity.magnitude > minVelocityToEscape)
                {
                    rb.gravityScale = 1f;
                    continue;
                }

                var directionToCenter = (Vector2)(transform.position - rb.transform.position);
                var distanceToCenter = directionToCenter.magnitude;

                if (distanceToCenter < stopThreshold)
                {
                    rb.MovePosition(transform.position);
                    rb.velocity = Vector2.zero;
                    Debug.Log("[sinkhole] ball reached center");
                    continue;
                }

                // Spiral movement 
                var currentAngle = Mathf.Atan2(rb.position.y - transform.position.y, rb.position.x - transform.position.x);
                currentAngle += spiralSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;

                var inwardPull = pullStrength * (distanceToCenter / pullRadius) * pullSpeedFactor;

                var newRadius = Mathf.Max(distanceToCenter - inwardPull * Time.fixedDeltaTime, stopThreshold);
                var newPosition = new Vector2(
                    transform.position.x + Mathf.Cos(currentAngle) * newRadius,
                    transform.position.y + Mathf.Sin(currentAngle) * newRadius
                );

                rb.MovePosition(newPosition);

                // Override gravity 
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopThreshold);
    }
}
