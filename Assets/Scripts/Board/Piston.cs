using UnityEngine;

public class Piston : MonoBehaviour
{
    [SerializeField] private float force = 10f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float maxHeight = 2f;
    [SerializeField] private GameObject body;
    [SerializeField] private float resetSpeed = 2f;
    [SerializeField] private ParticleSystem activationParticles;
    //[SerializeField] private AudioSource activationSound;
    //[SerializeField] private AudioSource resetSound;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool canActivate = true;
    private bool isActive;
    private float cooldownTimer = 0f;
    private bool isResetting;

    private void Start()
    {
        rb = body.GetComponent<Rigidbody2D>();
        startPos = body.transform.position;
    }

    private void Update()
    {
        if (!canActivate)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                canActivate = true;
                cooldownTimer = 0f;
            }
        }

        if (isActive && body.transform.position.y >= startPos.y + maxHeight)
        {
            Debug.Log("[piston] starting reset");
            //if (resetSound != null)
            //{
            //    resetSound.Play();
            //}

            rb.velocity = Vector2.zero;
            isActive = false;
            isResetting = true;
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
        rb.velocity = Vector2.up * force;

        if (activationParticles != null)
        {
            activationParticles.Play();
        }

        //if (activationSound != null)
        //{
        //    activationSound.Play();
        //}

        isActive = true;
        canActivate = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        Activate();
    }
}
