using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        onEnter?.Invoke();
    }
}
