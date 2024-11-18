using Events;
using UnityEngine;
using UnityEngine.Events;

public class Charger : ReceiverBase
{
    [SerializeField] private UnityEvent onCharged;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision) || !collision.TryGetComponent<Ball>(out var ball) || ball.IsCharged)
        {
            return;
        }

        base.OnTriggerEnter2D(collision);
    }

    protected override void OnExit(Collider2D collision)
    {
        GameManager.EventService.Dispatch<BallChargedEvent>();
        base.OnExit(collision);
        onCharged?.Invoke();
    }
}