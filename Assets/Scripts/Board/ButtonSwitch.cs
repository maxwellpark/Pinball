using UnityEngine;
using UnityEngine.Events;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] private float force = 10f;
    [SerializeField] private SpringJoint2D spring;
    [SerializeField] private UnityAction onPressed;

    private Rigidbody2D rb;
    private bool isPressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision) || isPressed)
        {
            return;
        }

        isPressed = true;
        rb.AddForce(Vector2.down * force, ForceMode2D.Impulse);
        onPressed?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!Utils.IsBallOrGhostBall(collision))
        {
            return;
        }

        isPressed = false;
    }
}
