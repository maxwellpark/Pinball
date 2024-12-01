using UnityEngine;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private float maxRadius = 2.0f;
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private float contractionSpeed = 1.5f;
    [SerializeField] private float stickinessFactor = 0.5f;
    [SerializeField] private Material ringMaterial;
    [SerializeField] private int lineSegments = 100;

    private CircleCollider2D playerCollider;
    //private LineRenderer lineRenderer;
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

        //if (lineRenderer != null)
        //{
        //    Destroy(lineRenderer.gameObject);
        //}

        //var obj = new GameObject("PlayerRing");
        //lineRenderer = obj.AddComponent<LineRenderer>();

        //lineRenderer.positionCount = lineSegments + 1;
        //lineRenderer.useWorldSpace = false;
        //lineRenderer.loop = true;

        //var angleStep = 360f / lineSegments;
        //var radius = playerCollider.radius;
        //for (var i = 0; i < lineSegments + 1; i++)
        //{
        //    var angle = i * angleStep;
        //    Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius, 0);
        //    lineRenderer.SetPosition(i, position);
        //}
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

    //private void OnDrawGizmos()
    //{
    //    if (playerCollider != null)
    //    {
    //        Gizmos.color = Color.magenta;
    //        Gizmos.DrawWireSphere(transform.position, playerCollider.radius);
    //    }
    //}
}
