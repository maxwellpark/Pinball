using Events;
using UnityEngine;
using UnityEngine.Events;

public class Discharger : ReceiverBase
{
    [SerializeField] private UnityEvent onCharged;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Ball>(out var ball) || !ball.IsCharged)
        {
            return;
        }

        base.OnTriggerEnter2D(collision);
    }

    //protected override void OnEnter(Collider2D collision)
    //{
    //    if (!collision.TryGetComponent<Ball>(out var ball) || !ball.IsCharged)
    //    {
    //        return;
    //    }

    //    base.OnEnter(collision);
    //}

    protected override void OnExit(Collider2D collision)
    {
        GameManager.EventService.Dispatch<BallDischargedEvent>();
        base.OnExit(collision);
        onCharged?.Invoke();
    }
}