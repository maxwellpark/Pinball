using UnityEngine;

public class TargetOuterRingTrigger : MonoBehaviour
{
    public System.Action<bool> OnTargetOuterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PlayerRingOuter))
        {
            Debug.Log("[ring] player outer entered target outer");
            OnTargetOuterTrigger?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PlayerRingOuter))
        {
            Debug.Log("[ring] player outer exited target outer");
            OnTargetOuterTrigger?.Invoke(false);
        }
    }
}
