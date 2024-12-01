using UnityEngine;

public class TargetRing : MonoBehaviour
{
    [SerializeField] private float minOuterRadius = 3f;
    [SerializeField] private float maxOuterRadius = 5f;
    [SerializeField] private float minInnerRadius = 2.5f;
    [SerializeField] private float maxInnerRadius = 4.5f;
    [SerializeField] private float innerRadiusOffset = 0.75f;
    [SerializeField] private float expandContractSpeed = 1.5f;
    [SerializeField] private Vector2 pauseDurationRange = new(0.25f, 1.0f);
    [SerializeField] private float speedFactor = 1.5f;

    private CircleCollider2D outerCollider;
    private CircleCollider2D innerCollider;
    private bool isPaused = false;
    private float pauseTimer = 0f;
    private float randomMovementFactor;
    private float targetOuterRadius;
    private float targetInnerRadius;

    private void Start()
    {
        var colliders = GetComponentsInChildren<CircleCollider2D>();
        outerCollider = colliders[0];
        innerCollider = colliders[1];

        outerCollider.isTrigger = true;
        innerCollider.isTrigger = true;

        targetOuterRadius = Random.Range(minOuterRadius, maxOuterRadius);
        targetInnerRadius = targetOuterRadius - innerRadiusOffset;
        //targetInnerRadius = targetOuterRadius - Random.Range(minInnerRadius, maxInnerRadius);

        randomMovementFactor = Random.Range(0.5f, 1.5f);
    }

    private void Update()
    {
        if (!isPaused)
        {
            var expansionAmount = expandContractSpeed * Time.deltaTime * randomMovementFactor;

            outerCollider.radius = Mathf.MoveTowards(outerCollider.radius, targetOuterRadius, expansionAmount);
            innerCollider.radius = Mathf.MoveTowards(innerCollider.radius, targetInnerRadius, expansionAmount * (minInnerRadius / minOuterRadius));

            if (Mathf.Approximately(outerCollider.radius, targetOuterRadius))
            {
                isPaused = true;
                pauseTimer = Random.Range(pauseDurationRange.x, pauseDurationRange.y);

                targetOuterRadius = Random.Range(minOuterRadius, maxOuterRadius);
                targetInnerRadius = targetOuterRadius - innerRadiusOffset;
                //targetInnerRadius = targetOuterRadius - Random.Range(minInnerRadius, maxInnerRadius);

                randomMovementFactor = Random.Range(0.5f, speedFactor);
            }
        }
        else
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                randomMovementFactor = Random.Range(0.5f, speedFactor);
            }
        }
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
