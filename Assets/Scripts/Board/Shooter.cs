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
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, startPoint + direction * range);
        }
    }

    private IEnumerator FireLaser()
    {
        Debug.Log("[shooter] start firing laser...");
        var startPoint = transform.position;
        var direction = transform.up;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint + direction * range);
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.enabled = true;

        var hits = Physics2D.RaycastAll(startPoint, direction, range);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<DestructibleBumper>(out var bumper))
            {
                bumper.StartDamageOverTime(damage, laserDuration);
            }
        }

        yield return new WaitForSeconds(laserDuration);

        Debug.Log("[shooter] stop firing laser");
        lineRenderer.enabled = false;
        Destroy(gameObject);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.up * range);
    //}
}
