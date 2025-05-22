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
    public class MinigameCancelledEvent : IEvent { }
    public class BallSavedEvent : IEvent { }
    public class BallStuckEvent : IEvent { }
    public class NewBallEvent : IEvent { }
    public class ActivePlungerChangedEvent : IEvent
    {
        public Plunger Plunger { get; }
        public ActivePlungerChangedEvent(Plunger plunger)
        {
            Plunger = plunger;
        }
    }
    public class BallChargedEvent : IEvent { }
    public class BallDischargedEvent : IEvent { }
    public class ShooterCreatedEvent : IEvent { }
    public class ShooterDestroyedEvent : IEvent { }
    public class BoardChangedEvent : IEvent
    {
        public BoardConfig Config { get; }
        public BoardChangedEvent(BoardConfig config)
        {
            Config = config;
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
    public class CamerasUpdatedEvent : IEvent { }
    public class AbilityChangedEvent : IEvent
    {
        public Ability Ability { get; }
        public AbilityChangedEvent(Ability ability)
        {
            Ability = ability;
        }
    }
    public class AbilityUsedEvent : AbilityChangedEvent
    {
        public int Uses { get; }
        public AbilityUsedEvent(Ability ability, int uses) : base(ability)
        {
            Uses = uses;
        }
    }
    public class FlipperReleasedEvent : IEvent
    {
        public Flipper Flipper { get; }
        public FlipperReleasedEvent(Flipper flipper)
        {
            Flipper = flipper;
        }
    }
}
