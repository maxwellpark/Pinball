using UnityEngine;

public class PlayerInnerRingTrigger : MonoBehaviour
{
    public System.Action<bool> OnPlayerInnerTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingInner))
        {
            Debug.Log("[ring] target inner entered player inner");
            OnPlayerInnerTrigger?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingInner))
        {
            Debug.Log("[ring] target inner exited player inner");
            OnPlayerInnerTrigger?.Invoke(false);
        }
    }
}
