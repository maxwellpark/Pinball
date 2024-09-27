using UnityEngine.Events;

namespace Events
{
    public interface IEvent { }
    public class ObjectCaughtEvent : IEvent { }
    public class ObjectMissedEvent : IEvent { }
    public class MinigameStartedEvent : IEvent
    {
        public UnityAction OnEnd { get; }
        public MinigameStartedEvent(UnityAction onEnd)
        {
            OnEnd = onEnd;
        }
    }
    public class MinigameEndedEvent : IEvent { }
    public class BallSavedEvent : IEvent { }
}
