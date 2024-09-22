using UnityEngine;
using UnityEngine.Events;

public class Charger : ReceiverBase
{
    [SerializeField] private UnityEvent onCharged;

    protected override void OnExit(Collider2D collision)
    {
        base.OnExit(collision);
        onCharged?.Invoke();
    }
}