using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class AreaInputTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode key = KeyCode.C;
    [SerializeField] private UnityEvent onUse;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        //Debug.Log("[ait] ball inside");

        if (Input.GetKeyDown(key))
        {
            Debug.Log($"[ait] {name} used");
            onUse?.Invoke();
        }
    }
}
