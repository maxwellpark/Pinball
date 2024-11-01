using UnityEngine;
using UnityEngine.Events;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] private Transform switchTop;
    [SerializeField] private UnityEvent onPressed;
    [SerializeField] private GameManager.Action action;
    [SerializeField] private float pressDepth = 0.2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float cooldownInSeconds = 2f;
    [Tooltip("Number of presses required to trigger the action")]
    [SerializeField] private int requiredPresses = 1;

    private bool isPressed;
    private float cooldownTimer;
    private Vector3 startPos;
    private Vector3 pressedPos;
    private int pressCount;

    private void Start()
    {
        startPos = switchTop.localPosition;
        pressedPos = startPos - new Vector3(0, pressDepth, 0);
        cooldownTimer = cooldownInSeconds;
    }

    private void Update()
    {
        if (cooldownTimer < cooldownInSeconds)
        {
            cooldownTimer += Time.deltaTime;
        }

        if (isPressed)
        {
            switchTop.localPosition = Vector3.Lerp(switchTop.localPosition, pressedPos, Time.deltaTime * moveSpeed);
        }
        else
        {
            switchTop.localPosition = Vector3.Lerp(switchTop.localPosition, startPos, Time.deltaTime * moveSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPressed || !Utils.IsBall(collision) || cooldownTimer < cooldownInSeconds)
        {
            return;
        }

        isPressed = true;
        cooldownTimer = 0f;
        pressCount++;
        Debug.Log($"[switch] pressed {pressCount} times");

        if (pressCount >= requiredPresses)
        {
            Debug.Log("[switch] press threshold reached; triggering action " + action);
            pressCount = 0;
            onPressed?.Invoke();
            GameManager.TriggerAction(action);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isPressed || !Utils.IsBall(collision))
        {
            return;
        }

        isPressed = false;
    }
}
