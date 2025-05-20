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

        if (flashOnSpeed && collision.relativeVelocity.magnitude >= flashSpeedThreshold)
        {
            StartFlashFrame();
        }
    }

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

    public void StartFlashFrame()
    {
        StartCoroutine(FlashFrame());
    }

    private IEnumerator FlashFrame()
    {
        Debug.Log("[ball] starting flash frame!");
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
    }
}
