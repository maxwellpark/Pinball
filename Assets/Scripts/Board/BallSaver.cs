using Events;
using UnityEngine;

public class BallSaver : Buff
{
    protected override void TriggerBehaviour(Collider2D collision)
    {
        GameManager.EventService.Dispatch<BallSavedEvent>();
    }
}
