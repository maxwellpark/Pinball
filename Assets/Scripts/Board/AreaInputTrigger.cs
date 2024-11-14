using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class AreaInputTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onUse;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        //Debug.Log("[ait] ball inside");

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("[ait] used");
            onUse?.Invoke();
        }
    }
}
