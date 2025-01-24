using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    [SerializeField] protected bool permanentlyActive;
    public bool IsActive { get; protected set; }
    protected virtual bool ShouldTrigger(Collider2D collision) => (permanentlyActive || IsActive) && collision.IsBall();

    protected abstract void TriggerBehaviour(Collider2D collision);

    protected virtual void Start()
    {
        if (TryGetComponent<BoxColliderDrawer>(out var drawer))
        {
            drawer.SetIsDrawing(() => permanentlyActive || IsActive);
        }
    }

    public virtual void Activate()
    {
        if (IsActive || permanentlyActive)
        {
            return;
        }

        NotificationManager.Notify($"{GetType()} activating...", 1f);
        IsActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ShouldTrigger(collision))
        {
            return;
        }

        Debug.Log($"{name} triggering effect...");
        TriggerBehaviour(collision);
        IsActive = false;
    }
}
