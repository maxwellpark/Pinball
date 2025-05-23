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
    // TODO: although we have descriptive names, we should key these more intuitively
    private static readonly string xboxLeftTriggerAxis = "9";
    private static readonly string xboxRightTriggerAxis = "10";
    private static readonly string xboxDPadVerticalAxis = "13";
    private static readonly string xboxDPadHorizontalAxis = "14";

    private static readonly string ps4LeftTriggerAxis = "11";
    private static readonly string ps4RightTriggerAxis = "12";
    private static readonly string ps4DPadVerticalAxis = "15";
    private static readonly string ps4DPadHorizontalAxis = "16";

    // TODO: don't assume only 1 type of controller connected at once?
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

        if (xboxOneControllerConnected && !isXboxOneControllerConnected)
        {
            Debug.Log("[input] Xbox One controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.XboxOne));
        }

        if (!xboxOneControllerConnected && isXboxOneControllerConnected)
        {
            Debug.Log("[input] Xbox One controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.XboxOne));
        }

        if (ps4ControllerConnected && !isPs4ControllerConnected)
        {
            Debug.Log("[input] PS4 controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.Ps4));
        }

        if (!ps4ControllerConnected && isPs4ControllerConnected)
        {
            Debug.Log("[input] PS4 controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.Ps4));
        }

        xboxOneControllerConnected = isXboxOneControllerConnected;
        ps4ControllerConnected = isPs4ControllerConnected;
    }

    public static bool IsButtonX() => Input.GetKeyDown(KeyCode.JoystickButton0);
    public static bool IsButtonA() => Input.GetKeyDown(KeyCode.JoystickButton1);
    public static bool IsButtonB() => Input.GetKeyDown(KeyCode.JoystickButton2);
    public static bool IsButtonY() => Input.GetKeyDown(KeyCode.JoystickButton3);
    public static bool IsLeftBumper() => Input.GetKeyDown(KeyCode.JoystickButton4);
    public static bool IsRightBumper() => Input.GetKeyDown(KeyCode.JoystickButton5);
    public static bool IsLeftTriggerOver() => IsAxisOverThreshold(xboxLeftTriggerAxis) || IsAxisOverThreshold(ps4LeftTriggerAxis);
    public static bool IsRightTriggerOver() => IsAxisOverThreshold(xboxRightTriggerAxis) || IsAxisOverThreshold(ps4RightTriggerAxis);

    public static bool IsDPadUp() => GetDPadVertical() > axisTreshold;
    public static bool IsDPadDown() => GetDPadVertical() < -axisTreshold;
    public static bool IsDPadLeft() => GetDPadHorizontal() < -axisTreshold;
    public static bool IsDPadRight() => GetDPadHorizontal() > axisTreshold;

    private static float GetDPadHorizontal()
    {
        if (ConnectedController == Controller.XboxOne)
        {
            return Input.GetAxis(xboxDPadHorizontalAxis);
        }
        else if (ConnectedController == Controller.Ps4)
        {
            return Input.GetAxis(ps4DPadHorizontalAxis);
        }
        return 0f;
    }

    private static float GetDPadVertical()
    {
        if (ConnectedController == Controller.XboxOne)
        {
            return Input.GetAxis(xboxDPadVerticalAxis);
        }
        else if (ConnectedController == Controller.Ps4)
        {
            return Input.GetAxis(ps4DPadVerticalAxis);
        }
        return 0f;
    }

    public static bool IsLeftKeyDown() => Utils.AnyKeysDown(LeftKeys);
    public static bool IsRightKeyDown() => Utils.AnyKeysDown(RightKeys);
    public static bool IsLeftKey() => Utils.AnyKeys(LeftKeys);
    public static bool IsRightKey() => Utils.AnyKeys(RightKeys);
    public static bool IsLeftDown() => IsLeftKeyDown() || IsLeftTriggerOver();
    public static bool IsRightDown() => IsRightKeyDown() || IsRightTriggerOver();
    public static bool IsLeft() => IsLeftKey() || IsLeftTriggerOver();
    public static bool IsRight() => IsRightKey() || IsRightTriggerOver();
    public static bool IsAxisOverThreshold(string axis) => Input.GetAxis(axis) >= axisTreshold;
}
