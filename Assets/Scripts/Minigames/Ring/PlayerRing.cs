using UnityEngine;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private float maxRadius = 2.0f;
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private float contractionSpeed = 1.5f;
    [SerializeField] private float stickinessFactor = 0.5f;
    [SerializeField] private float accelerationTime = 0.5f;
    [SerializeField] private float decelerationTime = 1f;

    private CircleCollider2D playerCollider;
    private float targetRadius;
    private float currentSpeed;
    private float currentRadius;

    private float accelerationProgress;
    private float decelerationProgress;

    private bool isExpanding;

    public bool IsInsideTarget { get; private set; }

    private void Awake()
    {
        playerCollider = GetComponent<CircleCollider2D>();
        playerCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        IsInsideTarget = false;
        currentRadius = playerCollider.radius;
        targetRadius = currentRadius;
        currentSpeed = contractionSpeed;
        isExpanding = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isExpanding)
            {
                isExpanding = true;
                accelerationProgress = 0f;
            }

            accelerationProgress += Time.deltaTime / accelerationTime;
            accelerationProgress = Mathf.Clamp01(accelerationProgress);
            currentSpeed = accelerationProgress == 1f ? expansionSpeed : 0f;
            //currentSpeed = Mathf.Lerp(contractionSpeed, expansionSpeed, accelerationProgress);
            targetRadius = maxRadius;
        }
        else
        {
            if (isExpanding)
            {
                decelerationProgress = 0f;
            }

            decelerationProgress += Time.deltaTime / decelerationTime;
            decelerationProgress = Mathf.Clamp01(decelerationProgress);
            currentSpeed = decelerationProgress == 1f ? contractionSpeed : 0f;
            //currentSpeed = Mathf.Lerp(expansionSpeed, contractionSpeed, decelerationProgress);
            targetRadius = minRadius;
            isExpanding = false;
        }

        if (Mathf.Abs(currentRadius - targetRadius) > 0.01f)
        {
            var speed = currentSpeed * Time.deltaTime;
            if (currentRadius < targetRadius)
            {
                currentRadius = Mathf.MoveTowards(currentRadius, targetRadius, speed);
            }
            else
            {
                currentRadius = Mathf.MoveTowards(currentRadius, targetRadius, speed * stickinessFactor);
            }

            playerCollider.radius = currentRadius;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingOuter))
        {
            var targetRing = other.GetComponentInParent<TargetRing>();
            IsInsideTarget = targetRing.IsPlayerInRing(playerCollider);
            Debug.Log("[ring] IsInsideTarget = " + IsInsideTarget);
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, playerCollider.radius);
        }
    }
}
