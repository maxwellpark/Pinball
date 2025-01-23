using Events;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Laser")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 10f;
    [SerializeField] private float laserWidth = 0.1f;
    [SerializeField] private float laserDuration = 0.2f;
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        GameManager.EventService.Dispatch<ShooterCreatedEvent>();
        StartCoroutine(FireLaser());
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

    private IEnumerator FireLaser()
    {
        Debug.Log("[shooter] start firing laser...");
        float elapsedTime = 0f;
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

    private void OnDestroy()
    {
        GameManager.EventService.Dispatch<ShooterDestroyedEvent>();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.up * range);
    //}
}
