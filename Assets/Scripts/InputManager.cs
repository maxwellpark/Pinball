using Events;
using System.Linq;
using UnityEngine;
using GM = GameManager;

/// <summary>
/// This uses the old input system. We might want to move to the new Unity input system at some point.
/// </summary>
public class InputManager : Singleton<InputManager>
{
    public enum Controller
    {
        None, XboxOne, Ps4,
    }

    private static bool xboxOneControllerConnected;
    private static bool ps4ControllerConnected;
    private static readonly float axisTreshold = 0.1f;
    private static readonly string xboxLeftTriggerAxis = "9";
    private static readonly string xboxRightTriggerAxis = "10";

    // https://www.reddit.com/r/Unity3D/comments/1syswe/ps4_controller_map_for_unity/
    private static readonly string ps4LeftTriggerAxis = "11";
    private static readonly string ps4RightTriggerAxis = "12";

    // Assume only 1 can be connected at 1 time for now 
    public static Controller ConnectedController
        => xboxOneControllerConnected ? Controller.XboxOne : ps4ControllerConnected ? Controller.Ps4 : Controller.None;

    public static readonly KeyCode[] LeftKeys = new[] { KeyCode.A, KeyCode.LeftArrow, KeyCode.S };
    public static readonly KeyCode[] RightKeys = new[] { KeyCode.D, KeyCode.RightArrow, KeyCode.S };

    private void Start()
    {
        var names = Input.GetJoystickNames();

        // TODO: Copied this from an old project before latest gen of consoles, so check what the names are for those?
        // Looks ugly but without the new input system this is the only reliable way?
        xboxOneControllerConnected = names.Any(n => n.Length == 19);
        ps4ControllerConnected = names.Any(n => n.Length == 33);
    }

    private void Update()
    {
        var names = Input.GetJoystickNames();
        var isXboxOneControllerConnected = names.Any(n => n.Length == 33);
        var isPs4ControllerConnected = names.Any(n => n.Length == 19);

        // Xbox One controller was connected last frame but not this frame 
        if (xboxOneControllerConnected && !isXboxOneControllerConnected)
        {
            Debug.Log("Xbox One controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.XboxOne));
        }

        // Xbox One controller was not connected last frame but now it is
        if (!xboxOneControllerConnected && isXboxOneControllerConnected)
        {
            Debug.Log("Xbox One controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.XboxOne));
        }

        // PS4 controller was connected last frame but not this frame 
        if (ps4ControllerConnected && !isPs4ControllerConnected)
        {
            Debug.Log("PS4 controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.Ps4));
        }

        // PS4 controller was not connected last frame but now it is
        if (!ps4ControllerConnected && isPs4ControllerConnected)
        {
            Debug.Log("PS4 controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.Ps4));
        }

        xboxOneControllerConnected = isXboxOneControllerConnected;
        ps4ControllerConnected = isPs4ControllerConnected;
        //Debug.Log("Connected controller: " + ConnectedController);
    }

    public static bool IsLeftTriggerOver()
    {
        //return ConnectedController == Controller.XboxOne
        //    ? IsAxisOverThreshold(xboxLeftTriggerAxis)
        //    : ConnectedController == Controller.Ps4 && IsAxisOverThreshold(ps4LeftTriggerAxis);

        return IsAxisOverThreshold(xboxLeftTriggerAxis) || IsAxisOverThreshold(ps4LeftTriggerAxis);
    }

    public static bool IsRightTriggerOver()
    {
        //return ConnectedController == Controller.XboxOne
        //    ? IsAxisOverThreshold(xboxRightTriggerAxis)
        //    : ConnectedController == Controller.Ps4 && IsAxisOverThreshold(ps4RightTriggerAxis);

        return IsAxisOverThreshold(xboxRightTriggerAxis) || IsAxisOverThreshold(ps4RightTriggerAxis);
    }

    public static bool IsLeftKeyDown()
    {
        return Utils.AnyKeysDown(LeftKeys);
    }

    public static bool IsRightKeyDown()
    {
        return Utils.AnyKeysDown(RightKeys);
    }

    public static bool IsLeftKey()
    {
        return Utils.AnyKeys(LeftKeys);
    }

    public static bool IsRightKey()
    {
        return Utils.AnyKeys(RightKeys);
    }

    public static bool IsLeftDown()
    {
        return IsLeftKeyDown() || IsLeftTriggerOver();
    }

    public static bool IsRightDown()
    {
        return IsRightKeyDown() || IsRightTriggerOver();
    }

    public static bool IsLeft()
    {
        return IsLeftKey() || IsLeftTriggerOver();
    }

    public static bool IsRight()
    {
        return IsRightKey() || IsRightTriggerOver();
    }

    public static bool IsAxisOverThreshold(string axis)
    {
        return Input.GetAxis(axis) >= axisTreshold;
    }
}
