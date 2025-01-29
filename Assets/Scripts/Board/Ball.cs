using Events;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // TODO: better abstraction for creating this effect on doing an action 
    [SerializeField] private GameObject actionParticlesPrefab;
    [SerializeField] private Color chargedColor = Color.yellow;
    [Tooltip("Time in seconds before the ball is considered stuck")]
    [SerializeField] private float stuckTimeInSeconds = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float stuckTimer;

    // Only used for debugging for now 
    private FlipperController flipperController;

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

    private void Update()
    {
        // Being kinematic indicates we're _supposed_ to be stationary, e.g. in a plunger/receiver. 
        // Velocity having some magnitude indicates we're moving i.e. not stuck. 
        if (rb.isKinematic || rb.velocity.sqrMagnitude > 0.01f)
        {
            stuckTimer = 0f;
        }
        else
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTimeInSeconds)
            {
                stuckTimer = 0f;
                GameManager.EventService.Dispatch<BallStuckEvent>();
                Debug.Log($"[ball] ball stuck at position {transform.position}.");
            }
        }
    }

    private void OnDestroy()
    {
        if (gameObject.IsBall())
        {
            GameManager.EventService.Remove<ShooterCreatedEvent>(Freeze);
            GameManager.EventService.Remove<ShooterDestroyedEvent>(Unfreeze);
        }
    }

    private void CreateActionParticles()
    {
        // Optional for now 
        if (actionParticlesPrefab != null)
        {
            Instantiate(actionParticlesPrefab, transform);
        }
    }

    public void Freeze()
    {
        if (rb == null || !TryGetComponent(out rb))
        {
            Debug.LogWarning("[ball] no rb found when attempting to freeze");
            return;
        }

        CreateActionParticles();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        rb.simulated = false;

        if (TryGetComponent<TrailRenderer>(out var trail))
        {
            trail.Clear();
        }
    }

    public void Unfreeze()
    {
        if (rb == null || !TryGetComponent(out rb))
        {
            Debug.LogWarning("[ball] no rb found when attempting to unfreeze");
            return;
        }

        //CreateActionParticles();
        rb.isKinematic = false;
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

    private void OnValidate()
    {
        stuckTimeInSeconds = Mathf.Max(0, stuckTimeInSeconds);
    }
}
