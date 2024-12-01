using UnityEngine;

public class PlayerOuterRingTrigger : MonoBehaviour
{
    public System.Action<bool> OnPlayerOuterTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingOuter))
        {
            Debug.Log("[ring] target outer entered player outer");
            OnPlayerOuterTrigger?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tags.TargetRingOuter))
        {
            Debug.Log("[ring] target outer exited player outer");
            OnPlayerOuterTrigger?.Invoke(false);
        }
    }
}
