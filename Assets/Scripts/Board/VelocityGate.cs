using UnityEngine;

public class VelocityGate : MonoBehaviour
{
    [SerializeField] private float velocityThreshold = 10f;
    [SerializeField] private Collider2D gateCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utils.IsBall(collision))
        {
            var ballRb = collision.GetComponent<Rigidbody2D>();
            var velocity = ballRb.velocity.magnitude;

            if (velocity >= velocityThreshold)
            {
                gateCollider.isTrigger = true;
                Debug.Log($"[gate] Ball entered. Velocity: {velocity}");
            }
            else
            {
                gateCollider.isTrigger = false;
                Debug.Log($"[gate] Ball blocked. Velocity: {velocity}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.IsBall(collision))
        {
            Debug.Log("[gate] Ball left");
            gateCollider.isTrigger = true;
        }
    }
}
