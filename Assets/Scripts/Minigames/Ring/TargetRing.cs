using UnityEngine;

public class TargetRing : MonoBehaviour
{
    [SerializeField] private float minOuterRadius = 3.0f;
    [SerializeField] private float maxOuterRadius = 5.0f;
    [SerializeField] private float minInnerRadius = 2.5f;
    [SerializeField] private float maxInnerRadius = 4.5f;
    [SerializeField] private float expandContractSpeed = 1.5f;

    private CircleCollider2D outerCollider;
    private CircleCollider2D innerCollider;

    private bool isExpanding = true;

    private void Start()
    {
        var colliders = GetComponentsInChildren<CircleCollider2D>();
        outerCollider = colliders[0];
        innerCollider = colliders[1];

        outerCollider.isTrigger = true;
        innerCollider.isTrigger = true;

        outerCollider.radius = Random.Range(minOuterRadius, maxOuterRadius);
        innerCollider.radius = Random.Range(minInnerRadius, maxInnerRadius);
        Debug.Log($"[ring] target ring outer radius: {outerCollider.radius} | inner radius: {innerCollider.radius}");
    }

    private void Update()
    {
        AdjustRingSize();
    }

    private void AdjustRingSize()
    {
        float expansionAmount = expandContractSpeed * Time.deltaTime;
        if (isExpanding)
        {
            outerCollider.radius += expansionAmount;
            innerCollider.radius += expansionAmount * (minInnerRadius / minOuterRadius);
        }
        else
        {
            outerCollider.radius -= expansionAmount;
            innerCollider.radius -= expansionAmount * (minInnerRadius / minOuterRadius);
        }

        if (outerCollider.radius >= maxOuterRadius || outerCollider.radius <= minOuterRadius)
        {
            isExpanding = !isExpanding; // Switch the state to contract/expand
        }

        outerCollider.radius = Mathf.Clamp(outerCollider.radius, minOuterRadius, maxOuterRadius);
        innerCollider.radius = Mathf.Clamp(innerCollider.radius, minInnerRadius, maxInnerRadius);
    }

    public bool IsPlayerInRing(CircleCollider2D playerCollider)
    {
        var playerRadius = playerCollider.radius;
        var outerRingRadius = outerCollider.radius;
        var innerRingRadius = innerCollider.radius;

        var isInsideOuter = playerRadius < outerRingRadius;
        var isOutsideInner = playerRadius > innerRingRadius;

        return isInsideOuter && isOutsideInner;
    }

    private void OnDrawGizmos()
    {
        if (outerCollider != null && innerCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, outerCollider.radius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, innerCollider.radius);
        }
    }
}
