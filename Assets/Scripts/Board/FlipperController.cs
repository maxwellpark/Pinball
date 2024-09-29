using UnityEngine;

public class FlipperController : MonoBehaviour
{
    [SerializeField] private HingeJoint2D leftFlipper;
    [SerializeField] private HingeJoint2D rightFlipper;

    [SerializeField] private float speed = 2000f;
    [SerializeField] private float returnSpeed = 1000f;

    [Header("Charge")]
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float baseForce = 10f;
    [SerializeField] private float maxForceMultiplier = 3f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color chargingColor = Color.yellow;
    [SerializeField] private Color fullyChargedColor = Color.red;

    private SpriteRenderer leftRenderer;
    private SpriteRenderer rightRenderer;

    private float leftChargeTime;
    private float rightChargeTime;

    private readonly KeyCode[] leftKeys = new[] { KeyCode.A, KeyCode.LeftArrow, KeyCode.S };
    private readonly KeyCode[] rightKeys = new[] { KeyCode.D, KeyCode.RightArrow, KeyCode.S };

    private void Start()
    {
        leftRenderer = leftFlipper.GetComponent<SpriteRenderer>();
        leftRenderer.color = defaultColor;
        rightRenderer = rightFlipper.GetComponent<SpriteRenderer>();
        rightRenderer.color = defaultColor;
    }

    private void Update()
    {
        if (GameManager.MinigameActive)
        {
            return;
        }

        var leftInput = Utils.AnyKeys(leftKeys);
        var rightInput = Utils.AnyKeys(rightKeys);

        UpdateFlipper(leftFlipper, leftInput, -speed, returnSpeed, ref leftChargeTime, leftRenderer);
        UpdateFlipper(rightFlipper, rightInput, speed, -returnSpeed, ref rightChargeTime, rightRenderer);
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

        var fullyExtended = isLeftFlipper ? Mathf.Abs(flipper.jointAngle - flipper.limits.min) < 0.5f : Mathf.Abs(flipper.jointAngle - flipper.limits.max) < 0.5f;

        // Holding a fully extended flipper can be used for a charge attack aka the "ball tuck" 
        if (fullyExtended)
        {
            chargeTime = Mathf.Clamp(chargeTime + Time.deltaTime, 0f, maxChargeTime);
        }
        else
        {
            chargeTime = Mathf.Clamp(chargeTime - Time.deltaTime, 0f, maxChargeTime);
        }

        if (chargeTime == 0)
        {
            spriteRenderer.color = defaultColor;
        }
        else
        {
            var t = chargeTime / maxChargeTime;
            spriteRenderer.color = Color.Lerp(chargingColor, fullyChargedColor, t);
        }

        //Debug.Log($"[flipper] motor speed: {speed} (charge time: {chargeTime})");
    }

    private float GetActiveMotorSpeed(float baseSpeed, float chargeTime)
    {
        var chargeMultiplier = Mathf.Lerp(1f, maxForceMultiplier, chargeTime / maxChargeTime);
        return baseSpeed * chargeMultiplier;
    }
}
