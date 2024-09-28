using UnityEngine.Events;

namespace Events
{
    public interface IEvent { }
    public class ObjectCaughtEvent : IEvent { }
    public class ObjectMissedEvent : IEvent { }
    public class MinigameStartedEvent : IEvent
    {
        public Minigame.Type Type { get; }
        public UnityAction OnEnd { get; }
        public MinigameStartedEvent(Minigame.Type type, UnityAction onEnd)
        {
            Type = type;
            OnEnd = onEnd;
        }
    }
    public class MinigameEndedEvent : IEvent { }
    public class BallSavedEvent : IEvent { }
    public class NewBallEvent : IEvent { }
}
