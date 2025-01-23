using Events;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Only used for debugging for now 
    private FlipperController flipperController;

    [SerializeField] private Color chargedColor = Color.yellow;

    private Color defaultColor;
    private bool isCharged;

    public bool IsCharged => isCharged;

    private void Awake()
    {
        defaultColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        flipperController = FindObjectOfType<FlipperController>();
    }

    private void Start()
    {
        // TODO: GhostBalls should inherit from Ball, rather than us using tags everywhere to differentiate
        if (gameObject.IsBall())
        {
            // Exclude GhostBalls 
            GameManager.EventService.Add<ShooterCreatedEvent>(Freeze);
            GameManager.EventService.Add<ShooterDestroyedEvent>(Unfreeze);
        }
    }

    public void Freeze()
    {
        if (rb == null && !rb.TryGetComponent(out rb))
        {
            Debug.LogWarning("[ball] no rb found when attempting to freeze");
            return;
        }

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        if (TryGetComponent<TrailRenderer>(out var trail))
        {
            trail.Clear();
        }
    }

    public void Unfreeze()
    {
        if (rb == null && !rb.TryGetComponent(out rb))
        {
            Debug.LogWarning("[ball] no rb found when attempting to unfreeze");
            return;
        }

        rb.simulated = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void Charge()
    {
        isCharged = true;
        sr.color = chargedColor;
        Debug.Log("[ball] charged!");
    }

    public void Discharge()
    {
        isCharged = false;
        sr.color = defaultColor;
        Debug.Log("[ball] discharged!");
    }

    //private void Update()
    //{
    //    Debug.Log($"[ball] linear velocity: {rb.velocity} | angular velocity: {rb.angularVelocity}");
    //}

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Only used for debugging for now 
        if (collision.gameObject.CompareTag(Tags.Flipper))
        {
            var vel = rb.velocity;
            var angVel = rb.angularVelocity;
            var lms = flipperController.LeftMotor.motorSpeed;
            var rms = flipperController.RightMotor.motorSpeed;

            Debug.Log($"[ball/flipper] lin. velocity: {vel} | ang. velocity: {angVel} | L motor speed: {lms} | R motor speed: {rms}");
        }
    }
}
