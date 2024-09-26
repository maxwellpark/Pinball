using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class BoxColliderDrawer : MonoBehaviour
{
    [SerializeField] private Color color = Color.green;

    public Func<bool> IsDrawing;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        IsDrawing = () => true;
    }

    public void SetIsDrawing(Func<bool> func)
    {
        IsDrawing = func;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null || IsDrawing == null || !IsDrawing())
        {
            return;
        }

        Gizmos.color = color;
        var bounds = boxCollider.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
