using Cinemachine;
using System;

public class CameraManager : Singleton<CameraManager>
{
    private static CinemachineLiveCameraShake liveCameraShake;
    private static CinemachineLiveCameraZoom liveCameraZoom;
    private static CinemachineVirtualCamera[] cameras;

    // TODO: not great we're exposing this, should listen to an event in CinemachineLiveCameraZoom instead.
    public static bool IsLiveCameraZooming;
    public static float LiveCameraOrthoSize => liveCameraZoom.OrthoSize;

    protected override void Awake()
    {
        base.Awake();
        cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        liveCameraShake = GetComponent<CinemachineLiveCameraShake>();
        liveCameraZoom = GetComponent<CinemachineLiveCameraZoom>();
    }

    public static void SetPriority(CinemachineVirtualCamera camera, int priority = 10)
    {
        Array.ForEach(cameras, c => c.Priority = 0);
        camera.Priority = priority;
    }

    public static void ShakeLiveCamera(CameraShakeSettings settings)
    {
        // TODO: get rid of this guard once camera shake is fully required 
        if (liveCameraShake == null)
        {
            return;
        }

        liveCameraShake.Shake(settings);
    }

    public static void ZoomLiveCamera(CameraZoomSettings settings)
    {
        liveCameraZoom.Zoom(settings);
    }
}
