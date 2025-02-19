using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class CinemachineLiveCameraZoom : CinemachineLiveCameraBehaviour
{
    // These 4 serialized fields are all for testing.
    // The numerics are just default values for when triggering zoom in the inspector.
    // Everything dynamic is done by passing CameraZoomSettings.
    [SerializeField]
    private float targetSize = 5f;

    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private bool start = false;

    [SerializeField]
    private bool reset = false;

    // Only used with 2D currently 
    public float OrthoSize
    {
        get => virtualCamera.m_Lens.OrthographicSize;
        private set => virtualCamera.m_Lens.OrthographicSize = value;
    }

    private float startingSize;

    private void Start()
    {
        start = false;
        reset = false;
    }

    protected override void OnCameraActivated(ICinemachineCamera active, ICinemachineCamera previous)
    {
        base.OnCameraActivated(active, previous);
        startingSize = OrthoSize;
        Debug.Log("[camera] starting ortho size: " + OrthoSize);
    }

    public void Zoom(CameraZoomSettings settings)
    {
        Zoom(settings.TargetSize, settings.Speed);
    }

    private void Zoom(float targetSize, float speed, Action action = null, bool resetAfter = false)
    {
        StartCoroutine(ZoomCameraRoutine(targetSize, speed, action, resetAfter));
    }

    public void ResetZoom()
    {
        OrthoSize = startingSize;
    }

    private IEnumerator ZoomCameraRoutine(float targetSize, float speed, Action action = null, bool resetAfter = false)
    {
        CameraManager.IsLiveCameraZooming = true;
        Debug.Log($"[camera] start zooming | target: {targetSize}, speed: {speed}");

        var direction = Mathf.Sign(targetSize - OrthoSize);

        // Adjust the OrthoSize until it reaches the target
        while ((direction > 0 && OrthoSize < targetSize) || (direction < 0 && OrthoSize > targetSize))
        {
            OrthoSize += direction * Time.deltaTime * speed;
            yield return null;
        }
        OrthoSize = targetSize;

        action?.Invoke();

        if (resetAfter)
        {
            ResetZoom();
        }
        CameraManager.IsLiveCameraZooming = false;
        Debug.Log("[camera] stop zooming");
    }


    private void Update()
    {
        if (start)
        {
            Zoom(targetSize, speed);
            start = false;
        }

        if (reset && virtualCamera != null)
        {
            ResetZoom();
            reset = false;
        }
    }

    private void OnValidate()
    {
        speed = Mathf.Max(1f, speed);
    }
}