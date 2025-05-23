using Cinemachine;
using UnityEngine;

public abstract class CinemachineLiveCameraBehaviour : MonoBehaviour
{
    protected CinemachineBrain cinemachineBrain;
    protected CinemachineVirtualCamera virtualCamera;
    public string CameraName => virtualCamera.Name;

    protected virtual void Awake()
    {
        cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    protected virtual void OnCameraActivated(ICinemachineCamera active, ICinemachineCamera previous)
    {
        Debug.Log($"[camera] CinemachineBrain OnCameraActivated fired");
        if (previous != null)
        {
            Debug.Log("[camera] previous cam: " + previous.Name);
        }

        if (active != null)
        {
            Debug.Log("[camera] next cam: " + active.Name);
        }
        else
        {
            return;
        }

        virtualCamera = active as CinemachineVirtualCamera;
    }

    private void OnDestroy()
    {
        cinemachineBrain.m_CameraActivatedEvent.RemoveListener(OnCameraActivated);
    }
}
