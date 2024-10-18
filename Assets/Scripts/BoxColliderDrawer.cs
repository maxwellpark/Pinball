using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class BoxColliderDrawer : MonoBehaviour
{
    [SerializeField] private Color color = Color.green;

    private Func<bool> isDrawing;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        isDrawing = () => true;
    }

    public void SetIsDrawing(Func<bool> func)
    {
        isDrawing = func;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null || isDrawing != null && !isDrawing())
        {
            return;
        }

        Gizmos.color = color;
        var bounds = boxCollider.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
