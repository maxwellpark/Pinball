namespace Events
{
    public interface IEvent { }
    public class ObjectCaughtEvent : IEvent { }
    public class ObjectMissedEvent : IEvent { }
    public class MinigameStartedEvent : IEvent { }
    public class MinigameEndedEvent : IEvent { }
}
