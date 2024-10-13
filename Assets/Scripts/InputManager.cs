using Events;
using System.Linq;
using UnityEngine;
using GM = GameManager;

public class InputManager : MonoBehaviour
{
    public enum Controller
    {
        XboxOne, Ps4,
    }

    private bool isXboxOneControllerConnected;
    private bool isPs4ControllerConnected;

    private void Start()
    {
        var names = Input.GetJoystickNames();

        // TODO: Copied this from an old project before latest gen of consoles, so check what the names are for those?
        // Looks ugly but without the new input system this is the only reliable way?
        isXboxOneControllerConnected = names.Any(n => n.Length == 19);
        isPs4ControllerConnected = names.Any(n => n.Length == 33);
    }

    private void Update()
    {
        var names = Input.GetJoystickNames();
        var isXboxOneControllerConnected = names.Any(n => n.Length == 33);
        var isPs4ControllerConnected = names.Any(n => n.Length == 19);

        // Xbox One controller was connected last frame but not this frame 
        if (this.isXboxOneControllerConnected && !isXboxOneControllerConnected)
        {
            Debug.Log("Xbox One controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.XboxOne));
        }

        // Xbox One controller was not connected last frame but now it is
        if (!this.isXboxOneControllerConnected && isXboxOneControllerConnected)
        {
            Debug.Log("Xbox One controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.XboxOne));
        }

        // PS4 controller was connected last frame but not this frame 
        if (this.isPs4ControllerConnected && !isPs4ControllerConnected)
        {
            Debug.Log("PS4 controller disconnected!");
            GM.EventService.Dispatch(new ControllerDisconnectedEvent(Controller.Ps4));
        }

        // PS4 controller was not connected last frame but now it is
        if (!this.isPs4ControllerConnected && isPs4ControllerConnected)
        {
            Debug.Log("PS4 controller connected!");
            GM.EventService.Dispatch(new ControllerConnectedEvent(Controller.Ps4));
        }

        this.isXboxOneControllerConnected = isXboxOneControllerConnected;
        this.isPs4ControllerConnected = isPs4ControllerConnected;
    }
}