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
