using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] private float drag = 1f;

    public bool IsColliding { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IsColliding = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision))
        {
            return;
        }

        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        ballRb.drag = drag;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IsColliding = false;
        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        ballRb.drag = 0f;
    }
}
