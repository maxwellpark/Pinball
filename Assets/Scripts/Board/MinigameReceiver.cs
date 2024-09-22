using UnityEngine;

public class MinigameReceiver : ReceiverBase
{
    protected override void OnEnter(Collider2D collision)
    {
        base.OnEnter(collision);
        GameManager.Instance.StartMinigame(OnEnd);
    }

    private void OnEnd()
    {
        isWaiting = false;
    }
}