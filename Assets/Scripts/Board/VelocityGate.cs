using UnityEngine;

public class VelocityGate : MonoBehaviour
{
    [SerializeField] private float velocityThreshold = 10f;
    [SerializeField] private Collider2D gateCollider;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gateCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utils.IsBall(collision))
        {
            var ballRb = collision.GetComponent<Rigidbody2D>();
            var velocity = ballRb.velocity.magnitude;
            //var contactPoint = collider.GetContacts(0).point;
            //var pointVelocity = ballRb.GetPointVelocity(contactPoint).magnitude;

            if (velocity >= velocityThreshold)
            {
                gateCollider.isTrigger = true;
                Debug.Log($"[gate] Ball entered. Velocity: {velocity}");

                var color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f);
            }
            else
            {
                gateCollider.isTrigger = false;
                Debug.Log($"[gate] Ball blocked. Velocity: {velocity}");

                // Ensure we reset the trigger after blocking the ball as we might not get an OnTriggerExit2D message 
                Invoke(nameof(OnBlocked), 0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.IsBall(collision))
        {
            Debug.Log("[gate] Ball left");
            gateCollider.isTrigger = true;

            var color = spriteRenderer.color;
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
        }
    }

    private void OnBlocked()
    {
        Debug.Log("[gate] OnBlocked invoked");
        gateCollider.isTrigger = true;
    }
}
