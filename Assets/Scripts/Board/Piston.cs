using Events;
using UnityEngine;

public class Piston : MonoBehaviour
{
    public enum Type
    {
        Automatic, LeftInput, RightInput,
    }

    [SerializeField] private Type type;
    [SerializeField] private float force = 10f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float maxHeight = 2f;
    [SerializeField] private GameObject body;
    [SerializeField] private float resetSpeed = 2f;
    [SerializeField] private ParticleSystem activationParticles;
    [SerializeField] private AudioClip activationSound;
    //[SerializeField] private AudioSource resetSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector2 startPos;
    private bool canActivate = true;
    private bool isActive;
    private float cooldownTimer = 0f;
    private bool isResetting;

    private void Start()
    {
        rb = body.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        startPos = body.transform.position;
        GameManager.EventService.Add<BoardChangedEvent>(OnBoardChanged);
    }

    private void OnBoardChanged(BoardChangedEvent evt)
    {
        if (audioSource != null && evt.Config.PistonSound != null)
        {
            activationSound = evt.Config.PistonSound;
        }
    }

    private void Update()
    {
        if (type == Type.LeftInput && InputManager.IsLeftDown() || type == Type.RightInput && InputManager.IsRightDown())
        {
            Activate();
        }

        if (!canActivate)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                canActivate = true;
                cooldownTimer = 0f;
            }
        }

        if (isActive)
        {
            var displacement = body.transform.position - (Vector3)startPos;
            var displacementAlongUp = Vector3.Dot(displacement, body.transform.up);

            if (displacementAlongUp >= maxHeight)
            {
                Debug.Log("[piston] starting reset");
                // if (resetSound != null)
                // {
                //     resetSound.Play();
                // }

                rb.velocity = Vector2.zero;
                isActive = false;
                isResetting = true;
            }
        }

        if (isResetting)
        {
            body.transform.position = Vector2.MoveTowards(body.transform.position, startPos, resetSpeed * Time.deltaTime);

            if (Vector2.Distance(body.transform.position, startPos) < 0.01f)
            {
                isResetting = false;
                body.transform.position = startPos;
                Debug.Log("[piston] fully reset");
            }
        }
    }

    public void Activate()
    {
        if (!canActivate || isActive)
        {
            return;
        }

        Debug.Log("[piston] activating");
        rb.velocity = transform.up * force;

        if (activationParticles != null)
        {
            activationParticles.Play();
        }

        if (audioSource != null && activationSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(activationSound);
        }

        isActive = true;
        canActivate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only if not using player input do we activate on collision 
        // i.e. it just gets activated automatically when the ball touches it. 
        if (type != Type.Automatic || !collision.IsBall())
        {
            return;
        }

        Activate();
    }

    private void OnDestroy()
    {
        GameManager.EventService.Remove<BoardChangedEvent>(OnBoardChanged);
    }
}
