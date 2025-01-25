using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsBall())
        {
            onEnter?.Invoke();
        }
        // TODO: this means Floor can't be generic, and only used as a KillFloor (which it is currently)
        else if (collision.IsGhostBall())
        {
            Destroy(collision.gameObject, 0.25f);
        }
    }
}
