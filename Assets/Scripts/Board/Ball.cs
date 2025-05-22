using Events;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // TODO: better abstraction for creating this effect on doing an action 
    [SerializeField] private GameObject actionParticlesPrefab;
    [SerializeField] private Color chargedColor = Color.yellow;
    [Tooltip("Time in seconds before the ball is considered stuck")]
    [SerializeField] private float stuckTimeInSeconds = 5f;
    [Header("Flash frame")]
    [SerializeField] private float flashTimeInSeconds = 0.2f;
    [SerializeField] private float flashSpeedThreshold = 20f;
    [SerializeField] private bool flashOnSpeed;
    [SerializeField] private Color flashColor = Color.yellow;
    [Header("Audio")]
    [SerializeField] private AudioSource bombSource;
    [SerializeField] private AudioClip bombSound;
    [SerializeField] private AudioSource flashSource;
    [SerializeField] private AudioClip flashSound;
    [SerializeField] private AudioSource collisionSource;
    [SerializeField] private AudioClip collisionSound;

    public AudioClip BombSound => bombSound;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float stuckTimer;

    private Color defaultColor;
    private bool isCharged;
    private bool inFlashFrame;

    public bool IsCharged => isCharged;

    private void Awake()
    {
        defaultColor = GetComponent<SpriteRenderer>().color;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // TODO: GhostBalls should inherit from Ball, rather than us using tags everywhere to differentiate
        if (gameObject.IsBall())
        {
            // Exclude GhostBalls 
            GameManager.EventService.Add<ShooterCreatedEvent>(Freeze);
            GameManager.EventService.Add<ShooterDestroyedEvent>(Unfreeze);
            GameManager.EventService.Add<FlipperReleasedEvent>(OnFlipperReleased);
        }
    }

    private void Update()
    {
        // TODO: handle stuck GhostBalls (destroy rather than reset pos?)
        if (!gameObject.IsBall())
        {
            return;
        }

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
                NotificationManager.Notify("Ball stuck!", 1.5f);
            }
        }

        // For testing 
        if (Input.GetMouseButtonDown(2))
        {
            StartFlashFrame();
        }

        //Debug.Log($"[ball] linear velocity: {rb.velocity} | angular velocity: {rb.angularVelocity}");
    }

    private void OnDestroy()
    {
        if (gameObject.IsBall())
        {
            GameManager.EventService.Remove<ShooterCreatedEvent>(Freeze);
            GameManager.EventService.Remove<ShooterDestroyedEvent>(Unfreeze);
            GameManager.EventService.Remove<FlipperReleasedEvent>(OnFlipperReleased);

            if (TryGetComponent<TrailRenderer>(out var trail))
            {
                trail.Clear();
            }
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

    // TODO: could listen for a OnBomb event instead or move all the bomb code into here.
    // It at least makes sense to have the clip config on the ball prefab itself. 
    public void PlayBombSound()
    {
        if (bombSource != null && bombSound != null)
        {
            bombSource.PlayOneShot(bombSound);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("[ball] colliding with " + collision.gameObject.name);
        //if (collisionSource != null && collisionSound != null)
        //{
        //    collisionSource.PlayOneShot(collisionSound);
        //}

        // TODO: probably need to scope this to only certain layers 
        if (flashOnSpeed && collision.relativeVelocity.magnitude >= flashSpeedThreshold)
        {
            StartFlashFrame();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Flipper) && collision.gameObject.TryGetComponent<Flipper>(out var flipper) && flipper != null)
        {
            var vel = rb.velocity;
            var angVel = rb.angularVelocity;
            var fc = flipper.Controller;
            var lms = fc.LeftMotor.motorSpeed;
            var rms = fc.RightMotor.motorSpeed;
            Debug.Log($"[flipper] EXIT lin. velocity: {vel} | ang. velocity: {angVel} | joint angle: {flipper.JointAngle} | L motor speed: {lms} | R motor speed: {rms} | L released: {fc.IsLeftReleased} | R released: {fc.IsRightReleased} | Time: {Time.time} | L charged: {fc.IsLeftCharged} | R charged: {fc.IsRightCharged} | L charge time: {fc.LeftChargeTime} | R charge time: {fc.RightChargeTime} | max charge time: {fc.MaxChargeTime}");

            //if (flipper.IsLeft && fc.IsLeftCharged && fc.IsLeftReleased || !flipper.IsLeft && fc.IsRightCharged && fc.IsRightReleased)
            //{
            //    StartFlashFrame();
            //}
        }
    }

    private void OnFlipperReleased(FlipperReleasedEvent evt)
    {
        var fc = evt.Flipper.Controller;
        var isLeft = evt.Flipper.IsLeft;

        // TODO: pass isCharged in the event args; shouldn't even need to expose the controller now 
        if (isLeft && fc.IsLeftCharged || !isLeft && fc.IsRightCharged)
        {
            StartFlashFrame();
        }
    }

    private void OnValidate()
    {
        stuckTimeInSeconds = Mathf.Max(0, stuckTimeInSeconds);
    }

    public void StartFlashFrame()
    {
        if (!inFlashFrame)
        {
            StartCoroutine(FlashFrame());
        }
    }

    private IEnumerator FlashFrame()
    {
        Debug.Log("[ball] starting flash frame!");
        inFlashFrame = true;
        Time.timeScale = 0f;
        //CreateActionParticles();

        if (flashSource != null && flashSound != null)
        {
            flashSource.PlayOneShot(flashSound);
        }

        var image = UIManager.Instance.FlashFrameImage;
        if (image != null)
        {
            image.gameObject.SetActive(true);
            image.color = flashColor;
        }

        yield return new WaitForSecondsRealtime(flashTimeInSeconds);
        Time.timeScale = 1f;
        Debug.Log("[ball] ending flash frame");

        if (image != null)
        {
            image.gameObject.SetActive(false);
            // TODO: restore to default color? 
        }
        inFlashFrame = false;
    }
}
