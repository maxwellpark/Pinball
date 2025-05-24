using UnityEngine;

public abstract class ProducerBase : MonoBehaviour
{
    [SerializeField] private bool onCollision;
    [SerializeField] private bool onDestroy;
    [SerializeField] private float onDestroyRadius = 5f;
    [SerializeField] private bool drawGizmo;
    [SerializeField] private Color gizmoColor = Color.yellow;

    // TODO: this Internal pattern that prevents overriding guards could be applied elsewhere now
    protected abstract void OnCollisionEnter2DInternal(Collision2D collision);
    protected abstract void OnDestroyInternal();

    protected virtual void Awake()
    {
        if (!onCollision && !onDestroy)
        {
            Debug.LogWarning($"[producer] {name} - onCollision and onDestroy are both false. Producer has no trigger.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!onCollision || !Utils.IsBall(collision))
        {
            return;
        }
        OnCollisionEnter2DInternal(collision);
    }

    private void OnDestroy()
    {
        if (!onDestroy)
        {
            return;
        }

        var ball = GameManager.Ball;
        if (ball == null || Vector3.Distance(transform.position, ball.transform.position) > onDestroyRadius)
        {
            return;
        }
        OnDestroyInternal();
    }

    private void OnValidate()
    {
        onDestroyRadius = Mathf.Max(0, onDestroyRadius);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo)
        {
            return;
        }
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, onDestroyRadius);
    }
}
