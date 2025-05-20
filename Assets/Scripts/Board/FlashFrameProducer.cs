using UnityEngine;

public class FlashFrameProducer : MonoBehaviour
{
    [SerializeField] private bool onCollision;
    [SerializeField] private bool onDestroy;
    [SerializeField] private float onDestroyRadius = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!onCollision || !Utils.IsBall(collision) || !collision.gameObject.TryGetComponent<Ball>(out var ball))
        {
            return;
        }
        ball.StartFlashFrame();
    }

    private void OnDestroy()
    {
        if (!onDestroy)
        {
            return;
        }

        var ball = GameManager.Ball;
        if (ball != null && Vector3.Distance(transform.position, ball.transform.position) <= onDestroyRadius)
        {
            ball.StartFlashFrame();
        }
    }

    private void OnValidate()
    {
        onDestroyRadius = Mathf.Max(0, onDestroyRadius);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, onDestroyRadius);
    //}
}
