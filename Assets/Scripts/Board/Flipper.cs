using UnityEngine;

public class Flipper : MonoBehaviour
{
    //[SerializeField] private float slowFactor = 0.5f;
    //[SerializeField] private float minVelocity = 2f;
    [SerializeField] private float drag = 1f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision))
        {
            return;
        }

        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        //var velocity = ballRb.velocity * slowFactor;

        //if (velocity.magnitude < minVelocity)
        //{
        //    velocity = velocity.normalized * minVelocity;
        //}
        //ballRb.velocity = velocity;

        ballRb.drag = drag;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
        ballRb.drag = 0f;
    }
}
