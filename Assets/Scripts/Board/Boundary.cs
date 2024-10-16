using UnityEditor;
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

#if UNITY_EDITOR
            Handles.Label(currentPoint, $"({currentPoint.x:F2}, {currentPoint.y:F2})");
#endif

            previousPoint = currentPoint;
        }

#if UNITY_EDITOR
        Handles.Label(transform.TransformPoint(points[0]), $"({transform.TransformPoint(points[0]).x:F2}, {transform.TransformPoint(points[0]).y:F2})");
#endif
    }

    void OnGUI()
    {
        if (edgeCollider == null || edgeCollider.points.Length == 0)
        {
            return;
        }

        var points = edgeCollider.points;
        foreach (var point in points)
        {
            var worldPoint = transform.TransformPoint(point);
            var screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
            screenPoint.y = Screen.height - screenPoint.y;

            GUI.Label(new Rect(screenPoint.x, screenPoint.y, 100, 20), $"({worldPoint.x:F2}, {worldPoint.y:F2})");
        }
    }
}
