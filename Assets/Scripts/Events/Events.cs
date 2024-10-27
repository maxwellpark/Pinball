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
    public class ActivePlungerChangedEvent : IEvent
    {
        public Plunger Plunger { get; }
        public ActivePlungerChangedEvent(Plunger plunger)
        {
            Plunger = plunger;
        }
    }
    public class ControllerEvent : IEvent
    {
        public InputManager.Controller Controller { get; }
        public ControllerEvent(InputManager.Controller controller)
        {
            Controller = controller;
        }
    }
    public class ControllerConnectedEvent : ControllerEvent
    {
        public ControllerConnectedEvent(InputManager.Controller controller) : base(controller)
        {
        }
    }
    public class ControllerDisconnectedEvent : ControllerEvent
    {
        public ControllerDisconnectedEvent(InputManager.Controller controller) : base(controller)
        {
        }
    }
}
