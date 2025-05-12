using UnityEngine;

public class MinigameReceiver : ReceiverBase
{
    [SerializeField] private Minigame.Type minigameType;

    protected override void OnEnter(Collider2D collision)
    {
        if (IsLocked)
        {
            return;
        }

        base.OnEnter(collision);
        GameManager.Instance.StartMinigame(minigameType, onEnd: OnEnd);
    }

    private void OnEnd()
    {
        isWaiting = false;
    }
}
