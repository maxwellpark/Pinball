using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [SerializeField] private HingeJoint2D leftFlipper;
    [SerializeField] private HingeJoint2D rightFlipper;
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
    }

    private void Update()
    {
        if (GameManager.MinigameActive)
        {
            return;
        }

        if ((InputManager.IsLeftDown() || InputManager.IsRightDown()) && audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }

        UpdateFlipper(leftFlipper, InputManager.IsLeft(), -speed, returnSpeed, ref leftChargeTime, leftRenderer);
        UpdateFlipper(rightFlipper, InputManager.IsRight(), speed, -returnSpeed, ref rightChargeTime, rightRenderer);
    }

    private void UpdateFlipper(
        HingeJoint2D flipper,
        bool isActive,
        float activeSpeed,
        float restingSpeed,
        ref float chargeTime,
        SpriteRenderer spriteRenderer)
    {
        var isLeftFlipper = flipper == leftFlipper;
        var motor = flipper.motor;
        var speed = isActive ? GetActiveMotorSpeed(activeSpeed, chargeTime) : restingSpeed;
        motor.motorSpeed = speed;
        flipper.motor = motor;

        if (isLeftFlipper)
        {
            LeftMotor = motor;
        }
        else
        {
            RightMotor = motor;
        }

        var fullyExtended = isLeftFlipper ? Mathf.Abs(flipper.jointAngle - flipper.limits.min) < 0.5f : Mathf.Abs(flipper.jointAngle - flipper.limits.max) < 0.5f;

        // Holding a fully extended flipper can be used for a charge attack aka the "ball tuck" 
        if (fullyExtended)
        {
            // TODO: pre/decay timers for individual flippers 
            chargeDecayTimer = 0f;

            preChargeTimer += Time.deltaTime;
            if (preChargeTimer >= preChargeDelay)
            {
                chargeTime = Mathf.Clamp(chargeTime + Time.deltaTime, 0f, maxChargeTime);
            }
        }
        else
        {
            chargeDecayTimer += Time.deltaTime;
            if (chargeDecayTimer >= chargeDecayDelay)
            {
                chargeTime = Mathf.Clamp(chargeTime - Time.deltaTime, 0f, maxChargeTime);
            }

            preChargeTimer = 0f;
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

        if (isActive)
        {
            Debug.Log($"[flipper] motor speed: {speed} (charge time: {chargeTime})");
        }
    }

    private float GetActiveMotorSpeed(float baseSpeed, float chargeTime)
    {
        var chargeMultiplier = Mathf.Lerp(1f, maxForceMultiplier, chargeTime / maxChargeTime);
        return baseSpeed * chargeMultiplier;
    }
}
