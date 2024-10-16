using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
[ExecuteAlways]
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
        if (polygonCollider == null || polygonCollider.pathCount == 0)
        {
            return;
        }

        Gizmos.color = color;

        for (var i = 0; i < polygonCollider.pathCount; i++)
        {
            var points = polygonCollider.GetPath(i);
            if (points.Length < 2)
            {
                continue;
            }

            var previous = transform.TransformPoint(points[0]);

            for (var j = 1; j < points.Length; j++)
            {
                var current = transform.TransformPoint(points[j]);
                Gizmos.DrawLine(previous, current);

                Handles.Label(current, $"({current.x:F2}, {current.y:F2})");
                previous = current;
            }

            Gizmos.DrawLine(previous, transform.TransformPoint(points[0]));

            Handles.Label(transform.TransformPoint(points[0]),
                $"({transform.TransformPoint(points[0]).x:F2}, {transform.TransformPoint(points[0]).y:F2})");
        }
    }
}
