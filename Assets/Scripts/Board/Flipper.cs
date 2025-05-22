using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] private float drag = 1f;

    private HingeJoint2D joint;
    private FlipperController controller;

    public HingeJoint2D Joint => joint;
    public float JointAngle => joint.jointAngle;
    public FlipperController Controller => controller;
    public bool IsColliding { get; private set; }
    public bool IsLeft { get; private set; }

    private void Awake()
    {
        joint = GetComponent<HingeJoint2D>();
        controller = GetComponentInParent<FlipperController>();
        IsLeft = controller.IsLeftFlipper(joint);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.IsBall())
        {
            IsColliding = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.IsBallOrGhostBall())
        {
            return;
        }

        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        ballRb.drag = drag;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.IsBall())
        {
            IsColliding = false;
        }

        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        ballRb.drag = 0f;
    }
}
