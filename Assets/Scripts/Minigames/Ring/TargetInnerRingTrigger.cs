using UnityEngine;

public class TargetInnerRingTrigger : MonoBehaviour
{
    public System.Action<bool> OnTargetInnerTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.PlayerRingInner))
        {
            Debug.Log("[ring] player inner entered target inner");
            OnTargetInnerTrigger?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.PlayerRingInner))
        {
            Debug.Log("[ring] player inner exited target inner");
            OnTargetInnerTrigger?.Invoke(false);
        }
    }
}
