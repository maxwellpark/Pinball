using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(EdgeCollider2D))]
public class Boundary : MonoBehaviour
{
    private EdgeCollider2D edgeCollider;

    void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    void OnDrawGizmos()
    {
        if (edgeCollider == null || edgeCollider.points.Length < 2)
        {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.5f);

        var points = edgeCollider.points;
        var previousPoint = transform.TransformPoint(points[0]);

        for (int i = 1; i < points.Length; i++)
        {
            var currentPoint = transform.TransformPoint(points[i]);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}
