using UnityEngine;

public class Piston : MonoBehaviour
{
    [SerializeField] private float force = 10f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float maxHeight = 2f;
    [SerializeField] private GameObject body;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool canActivate = true;
    private bool isActive;
    private float cooldownTimer = 0f;

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
            ResetState();
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
        isActive = true;
        canActivate = false;
    }

    private void ResetState()
    {
        Debug.Log("[piston] resetting");
        rb.velocity = Vector2.zero;
        body.transform.position = startPos;
        isActive = false;
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
