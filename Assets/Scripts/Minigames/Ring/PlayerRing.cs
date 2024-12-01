using UnityEngine;

public class PlayerRing : MonoBehaviour
{
    [SerializeField] private float maxRadius = 2.0f;
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float expansionSpeed = 2f;
    [SerializeField] private float contractionSpeed = 2f;

    private CircleCollider2D playerCollider;

    private void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();
        playerCollider.isTrigger = true;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Expand();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            Contract();
        }
    }

    private void Expand()
    {
        if (playerCollider.radius < maxRadius)
        {
            playerCollider.radius += expansionSpeed * Time.deltaTime;
        }
    }

    private void Contract()
    {
        if (playerCollider.radius > minRadius)
        {
            playerCollider.radius -= contractionSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("TargetRingOuter"))
        {
            var targetRing = other.GetComponentInParent<TargetRing>();
            if (targetRing.IsPlayerInRing(playerCollider))
            {
                Debug.Log("Player ring is inside the target ring!");
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
