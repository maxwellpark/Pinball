using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.IsBall(other))
        {
            Debug.Log("[camera] CameraTrigger OnTriggerEnter2D with " + other.name);
            vcam.Follow = other.transform;
            CameraManager.SetPriority(vcam);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.IsBall(other))
        {
            Debug.Log("[camera] CameraTrigger OnTriggerExit2D with " + other.name);
            // TODO: OnExit vcam follow?
        }
    }
}
