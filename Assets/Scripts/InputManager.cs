using Events;
using System.Linq;
using UnityEngine;
using GM = GameManager;

public class InputManager : MonoBehaviour
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
    private static readonly string ps4LeftTriggerAxis = "11";
    private static readonly string ps4RightTriggerAxis = "12";

    // Assume only 1 can be connected at 1 time 
    public static Controller ConnectedController
        => xboxOneControllerConnected ? Controller.XboxOne : ps4ControllerConnected ? Controller.Ps4 : Controller.None;

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
        Debug.Log("Connected controller: " + ConnectedController);
    }

    public static bool IsLeftTriggerDown()
    {
        return ConnectedController == Controller.XboxOne
            ? IsAxisOverThreshold(xboxLeftTriggerAxis)
            : ConnectedController == Controller.Ps4 && IsAxisOverThreshold(ps4LeftTriggerAxis);
    }

    public static bool IsRightTriggerDown()
    {
        return ConnectedController == Controller.XboxOne
            ? IsAxisOverThreshold(xboxRightTriggerAxis)
            : ConnectedController == Controller.Ps4 && IsAxisOverThreshold(ps4RightTriggerAxis);
    }

    public static bool IsAxisOverThreshold(string axis)
    {
        return Input.GetAxis(axis) >= axisTreshold;
    }
}
