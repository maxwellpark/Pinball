using Events;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum Type { Laser, GhostBalls };

    [SerializeField] private Type type;

    [Header("General")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float durationInSeconds = 10f;

    [Header("Laser")]
    [SerializeField] private float laserDamage = 50f;
    [SerializeField] private float laserRange = 10f;
    [SerializeField] private float laserWidth = 1f;
    [SerializeField] private float cameraShakeAmplitude = 2f;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Ghost balls")]
    [SerializeField] private float cooldownInSeconds = 0.25f;
    [SerializeField] private float force = 100f;

    private void Start()
    {
        GameManager.EventService.Dispatch<ShooterCreatedEvent>();
        CameraManager.ShakeLiveCamera(new CameraShakeSettings(cameraShakeAmplitude, durationInSeconds));

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
            lineRenderer.SetPosition(1, startPoint + direction * laserRange);
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
        }
    }

    private IEnumerator ShootLaser()
    {
        Debug.Log("[shooter] start firing laser...");
        lineRenderer.enabled = true;
        var elapsedTime = 0f;
        while (elapsedTime < durationInSeconds)
        {
            elapsedTime += Time.deltaTime;

            var hits = Physics2D.CircleCastAll(transform.position, laserWidth / 2f, transform.up, laserRange);
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<DestructibleBumper>(out var bumper))
                {
                    bumper.TakeDamage(laserDamage * Time.deltaTime);
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
        Debug.Log("[shooter] start shooting ghost balls...");
        lineRenderer.enabled = false;

        var elapsedTime = 0f;
        while (elapsedTime < durationInSeconds)
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
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * laserRange);
            Gizmos.DrawWireSphere(transform.position + transform.up * laserRange, laserWidth / 2f);
        }
        else if (type == Type.GhostBalls)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * laserRange);
        }
    }
}
