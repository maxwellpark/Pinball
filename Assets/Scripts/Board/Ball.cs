using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    // Only used for debugging for now 
    private FlipperController flipperController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flipperController = FindObjectOfType<FlipperController>();
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
