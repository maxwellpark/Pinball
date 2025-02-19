using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LiveCameraZoomTrigger : MonoBehaviour
{
    [SerializeField] private CameraZoomSettings enterZoom;
    //[SerializeField] private CameraZoomSettings exitZoom;
    [SerializeField] private bool resetOnExit;

    private float startingSize;
    private float startingSpeed;

    private void Start()
    {
        if (enterZoom == null/* && exitZoom == null*/)
        {
            Debug.LogWarning($"[camera] zoom trigger {name} - all settings are null");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enterZoom != null && Utils.IsBall(other))
        {
            Debug.LogWarning($"[camera] zoom trigger {name} - zooming on enter");

            // TODO: handle live camera changing during a zoom coroutine? 
            startingSize = CameraManager.LiveCameraOrthoSize;
            startingSpeed = enterZoom.Speed;
            CameraManager.ZoomLiveCamera(enterZoom);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.IsBall(other))
        {
            //if (exitZoom != null)
            //{
            //    CameraManager.ZoomLiveCamera(exitZoom);
            //}

            if (resetOnExit)
            {
                Debug.LogWarning($"[camera] zoom trigger {name} - resetting zoom on exit");
                CameraManager.ZoomLiveCamera(new(startingSize, startingSpeed));
            }
        }
    }
}
