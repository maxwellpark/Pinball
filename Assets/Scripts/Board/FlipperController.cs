using Events;
using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [SerializeField] private Flipper leftFlipper;
    [SerializeField] private Flipper rightFlipper;
    [SerializeField] private AudioClip sound;

    [SerializeField] private float speed = 2000f;
    [SerializeField] private float returnSpeed = 1000f;

    [Header("Charge")]
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float maxForceMultiplier = 3f;
    [SerializeField] private float preChargeDelay = 1f;
    [SerializeField] private float chargeDecayDelay = 2f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color chargingColor = Color.yellow;
    [SerializeField] private Color fullyChargedColor = Color.red;

    private SpriteRenderer leftRenderer;
    private SpriteRenderer rightRenderer;
    private AudioSource audioSource;

    private float leftChargeTime;
    private float rightChargeTime;
    private float preChargeTimer;
    private float chargeDecayTimer;
    private readonly float chargeBufferTime = 1f;

    private static readonly KeyCode[] chargeKeyCodes = new[] { KeyCode.X, KeyCode.JoystickButton2 };

    private float leftReleaseTime;
    private float rightReleaseTime;
    private bool isLeftReleased;
    private bool isRightReleased;
    private readonly float releaseBufferTime = 1f;
    public bool IsLeftReleased => /*isLeftReleased && */Time.time - leftReleaseTime <= releaseBufferTime;
    public bool IsRightReleased => /*isRightReleased && */Time.time - rightReleaseTime <= releaseBufferTime;
    public bool IsLeftCharged => maxChargeTime - leftChargeTime <= chargeBufferTime;
    public bool IsRightCharged => maxChargeTime - rightChargeTime <= chargeBufferTime;
    public float LeftChargeTime => leftChargeTime;
    public float RightChargeTime => rightChargeTime;
    public float MaxChargeTime => maxChargeTime;

    // Only used for debugging for now 
    public JointMotor2D LeftMotor { get; private set; }
    public JointMotor2D RightMotor { get; private set; }

    private void Start()
    {
        leftRenderer = leftFlipper.GetComponent<SpriteRenderer>();
        leftRenderer.color = defaultColor;
        rightRenderer = rightFlipper.GetComponent<SpriteRenderer>();
        rightRenderer.color = defaultColor;
        audioSource = GetComponent<AudioSource>();
        GameManager.EventService.Add<BoardChangedEvent>(OnBoardChanged);
    }

    private void OnBoardChanged(BoardChangedEvent evt)
    {
        if (audioSource != null && evt.Config.FlipperSound != null)
        {
            sound = evt.Config.FlipperSound;
        }
    }

    private void Update()
    {
        if (GameManager.MinigameActive)
        {
            return;
        }

        if ((InputManager.IsLeftDown() || InputManager.IsRightDown()) && audioSource != null && sound != null)
        {
            Debug.Log($"[flipper] playing {sound} clip");
            audioSource.PlayOneShot(sound);
        }

        UpdateCharge(leftFlipper, Utils.AnyKeys(chargeKeyCodes), ref leftChargeTime, leftRenderer);
        UpdateCharge(rightFlipper, Utils.AnyKeys(chargeKeyCodes), ref rightChargeTime, rightRenderer);

        UpdateFlipper(leftFlipper, InputManager.IsLeft(), -speed, returnSpeed, ref leftChargeTime, ref isLeftReleased, ref leftReleaseTime);
        UpdateFlipper(rightFlipper, InputManager.IsRight(), speed, -returnSpeed, ref rightChargeTime, ref isRightReleased, ref rightReleaseTime);
    }

    private void UpdateCharge(
        Flipper flipper,
        bool isCharging,
        ref float chargeTime,
        SpriteRenderer spriteRenderer)
    {
        // Holding a fully extended flipper can be used for a charge attack aka the "ball tuck" 
        //var isLeftFlipper = flipper == leftFlipper;
        //var fullyExtended = isLeftFlipper ? Mathf.Abs(flipper.jointAngle - flipper.limits.min) < 0.5f : Mathf.Abs(flipper.jointAngle - flipper.limits.max) < 0.5f;

        if (isCharging)
        {
            // TODO: pre/decay timers for individual flippers 
            chargeDecayTimer = 0f;
            preChargeTimer += Time.deltaTime;
            if (preChargeTimer >= preChargeDelay)
            {
                chargeTime = Mathf.Clamp(chargeTime + Time.deltaTime, 0f, maxChargeTime);
            }
            //Debug.Log($"[flipper] {flipper} CHARGING | chargeTime: {chargeTime}, preChargeTimer: {preChargeTimer}, chargeDecayTimer: {chargeDecayTimer}");
        }
        else
        {
            chargeDecayTimer += Time.deltaTime;
            if (chargeDecayTimer >= chargeDecayDelay)
            {
                chargeTime = Mathf.Clamp(chargeTime - Time.deltaTime, 0f, maxChargeTime);
            }

            preChargeTimer = 0f;
            //Debug.Log($"[flipper] {flipper} NOT CHARGING | chargeTime: {chargeTime}, preChargeTimer: {preChargeTimer}, chargeDecayTimer: {chargeDecayTimer}");
        }

        if (chargeTime == 0f)
        {
            spriteRenderer.color = defaultColor;
        }
        else
        {
            var t = chargeTime / maxChargeTime;
            spriteRenderer.color = Color.Lerp(chargingColor, fullyChargedColor, t);
        }
    }

    private void UpdateFlipper(
        Flipper flipper,
        bool isActive,
        float activeSpeed,
        float restingSpeed,
        ref float chargeTime,
        ref bool isReleased,
        ref float releaseTime)
    {
        var motor = flipper.Joint.motor;
        var speed = isActive ? GetActiveMotorSpeed(activeSpeed, chargeTime) : restingSpeed;
        motor.motorSpeed = speed;
        flipper.Joint.motor = motor;

        var isLeftFlipper = flipper == leftFlipper;
        if (isLeftFlipper)
        {
            LeftMotor = motor;
        }
        else
        {
            RightMotor = motor;
        }

        //Debug.Log($"[flipper] {flipper.name} isActive: {isActive}");
        if (isActive)
        {
            //Debug.Log($"[flipper] motor speed: {speed} (charge time: {chargeTime})");
            isReleased = false;
            Debug.Log($"[flipper] {flipper.name} isReleased set to: {isReleased}");
        }

        if (!isActive && !isReleased /* && Mathf.Abs(flipper.jointAngle) > 0.1f*/)
        {
            isReleased = true;
            Debug.Log($"[flipper] {flipper.name} isReleased set to: {isReleased}");
            releaseTime = Time.time;
            GameManager.EventService.Dispatch(new FlipperReleasedEvent(flipper));
            Debug.Log($"[flipper] {flipper.name} new release time: {Time.time}");
        }
    }

    private float GetActiveMotorSpeed(float baseSpeed, float chargeTime)
    {
        var chargeMultiplier = Mathf.Lerp(1f, maxForceMultiplier, chargeTime / maxChargeTime);
        return baseSpeed * chargeMultiplier;
    }

    private void OnDestroy()
    {
        GameManager.EventService.Remove<BoardChangedEvent>(OnBoardChanged);
    }

    public bool IsLeftFlipper(HingeJoint2D joint)
    {
        return joint == leftFlipper;
    }
}
