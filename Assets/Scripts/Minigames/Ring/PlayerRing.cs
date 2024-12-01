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
    [SerializeField] private Material ringMaterial;

    private CircleCollider2D playerCollider;
    private float targetRadius;
    private float currentSpeed;
    private float currentRadius;

    private float accelerationProgress;
    private float decelerationProgress;

    private bool isExpanding;

    private void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();
        playerCollider.isTrigger = true;
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
            currentSpeed = Mathf.Lerp(contractionSpeed, expansionSpeed, accelerationProgress);
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
            currentSpeed = Mathf.Lerp(expansionSpeed, contractionSpeed, decelerationProgress);
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, playerCollider.radius);
        }
    }
}
