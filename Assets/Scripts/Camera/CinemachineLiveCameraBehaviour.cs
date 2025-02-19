using Cinemachine;
using System.Linq;
using UnityEngine;

public abstract class CinemachineLiveCameraBehaviour : MonoBehaviour
{
    protected CinemachineBrain cinemachineBrain;
    protected CinemachineVirtualCamera virtualCamera;

    protected virtual void Awake()
    {
        cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
        UpdateActiveCamera();
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

    // TODO: this should be redundant now we listen for cinemachine events? 
    protected virtual void UpdateActiveCamera()
    {
        if (cinemachineBrain.ActiveVirtualCamera == null)
        {
            return;
        }

        virtualCamera = GetLiveVirtualCamera();
    }

    public CinemachineVirtualCamera GetLiveVirtualCamera()
    {
        if (cinemachineBrain.ActiveVirtualCamera == null)
        {
            return default;
        }

        // Select child camera if the active camera is a state driven camera 
        if (cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.TryGetComponent<CinemachineStateDrivenCamera>(out var stateDrivenCamera))
        {
            return stateDrivenCamera.ChildCameras.FirstOrDefault(vcam => CinemachineCore.Instance.IsLive(vcam)) as CinemachineVirtualCamera;
        }

        return cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }
}