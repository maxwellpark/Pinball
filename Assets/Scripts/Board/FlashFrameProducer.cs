using UnityEngine;

public class FlashFrameProducer : ProducerBase
{
    protected override void OnCollisionEnter2DInternal(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Ball>(out var ball))
        {
            // TODO: this probably should be checked in the base class 
            //Debug.LogError("[producer] - ball not found on collision " + collision.gameObject.name);
            return;
        }

        ball.StartFlashFrame();
    }

    protected override void OnDestroyInternal()
    {
        var ball = GameManager.Ball;
        if (ball == null)
        {
            //Debug.LogError("[producer] - ball not found on destroy");
            return;
        }

        ball.StartFlashFrame();
    }
}
