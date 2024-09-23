using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonBoundary : MonoBehaviour
{
    [SerializeField] private Color color = Color.green;
    private PolygonCollider2D polygonCollider;

    private void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void OnDrawGizmos()
    {
        if (polygonCollider.pathCount == 0)
        {
            return;
        }

        Gizmos.color = color;

        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            var points = polygonCollider.GetPath(i);
            if (points.Length < 2)
            {
                continue;
            }

            var previous = transform.TransformPoint(points[0]);

            for (int j = 1; j < points.Length; j++)
            {
                var current = transform.TransformPoint(points[j]);
                Gizmos.DrawLine(previous, current);
                previous = current;
            }

            Gizmos.DrawLine(previous, transform.TransformPoint(points[0]));
        }
    }
}
