using UnityEngine;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private float maxRadius = 2.0f;
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private float contractionSpeed = 1.5f;
    [SerializeField] private float stickinessFactor = 0.5f;

    private CircleCollider2D playerCollider;
    private float targetRadius;
    private float currentSpeed;

    private void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();
        playerCollider.isTrigger = true;
        targetRadius = playerCollider.radius;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            targetRadius = maxRadius;
            currentSpeed = expansionSpeed;
        }
        else
        {
            targetRadius = minRadius;
            currentSpeed = contractionSpeed;
        }

        if (Mathf.Abs(playerCollider.radius - targetRadius) > 0.01f)
        {
            var speed = currentSpeed * Time.deltaTime;
            if (playerCollider.radius < targetRadius)
            {
                playerCollider.radius = Mathf.MoveTowards(playerCollider.radius, targetRadius, speed);
            }
            else
            {
                playerCollider.radius = Mathf.MoveTowards(playerCollider.radius, targetRadius, speed * stickinessFactor);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingOuter))
        {
            var targetRing = other.GetComponentInParent<TargetRing>();
            if (targetRing.IsPlayerInRing(playerCollider))
            {
                Debug.Log("[ring] player ring is inside");
            }
            else
            {
                Debug.Log("[ring] player ring is outside");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, playerCollider.radius);
        }
    }
}
