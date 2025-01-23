using Events;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum Type { Laser, GhostBalls };

    [SerializeField] private Type type;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Laser")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 10f;
    [SerializeField] private float laserWidth = 0.1f;
    [SerializeField] private float laserDuration = 0.2f;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Ghost balls")]
    [SerializeField] private float cooldownInSeconds = 0.25f;
    [SerializeField] private float force = 100f;

    private void Start()
    {
        GameManager.EventService.Dispatch<ShooterCreatedEvent>();

        if (type == Type.Laser)
        {
            StartCoroutine(ShootLaser());
        }
        else if (type == Type.GhostBalls)
        {
            StartCoroutine(ShootGhostBalls());
        }
    }

    private void Update()
    {
        var rotationInput = 0f;
        if (InputManager.IsLeft())
        {
            rotationInput = 1f;
        }
        else if (InputManager.IsRight())
        {
            rotationInput = -1f;
        }

        transform.Rotate(Vector3.forward, rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            var startPoint = transform.position;
            var direction = transform.up;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, startPoint + direction * range);
        }
    }

    private IEnumerator ShootLaser()
    {
        Debug.Log("[shooter] start firing laser...");
        lineRenderer.enabled = true;
        var elapsedTime = 0f;
        while (elapsedTime < laserDuration)
        {
            elapsedTime += Time.deltaTime;

            var hits = Physics2D.RaycastAll(transform.position, transform.up, range);
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<DestructibleBumper>(out var bumper))
                {
                    bumper.TakeDamage(damage * Time.deltaTime);
                }
            }

            yield return null;
        }

        Debug.Log("[shooter] stopped firing laser");
        lineRenderer.enabled = false;
        Destroy(gameObject);
    }

    private IEnumerator ShootGhostBalls()
    {
        Debug.Log("[shooter] start shooting shot balls...");
        lineRenderer.enabled = false;

        var elapsedTime = 0f;
        while (elapsedTime < laserDuration)
        {
            var ghostBall = GameManager.CreateGhostBall();
            if (ghostBall != null && ghostBall.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.AddForce(transform.up * force, ForceMode2D.Impulse);
            }

            elapsedTime += cooldownInSeconds;
            yield return new WaitForSeconds(cooldownInSeconds);
        }

        Debug.Log("[shooter] stopped shooting ghost balls");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.EventService.Dispatch<ShooterDestroyedEvent>();
    }

    private void OnDrawGizmos()
    {
        if (type == Type.Laser)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * range);
    }
}
