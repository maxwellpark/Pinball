public class CameraManager : Singleton<CameraManager>
{
    private static CinemachineLiveCameraShake liveCameraShake;
    private static CinemachineLiveCameraZoom liveCameraZoom;

    // TODO: not great we're exposing this, should listen to an event in CinemachineLiveCameraZoom instead.
    public static bool IsLiveCameraZooming;
    public static float LiveCameraOrthoSize => liveCameraZoom.OrthoSize;

    protected override void Awake()
    {
        base.Awake();
        liveCameraShake = GetComponent<CinemachineLiveCameraShake>();
        liveCameraZoom = GetComponent<CinemachineLiveCameraZoom>();
    }

    public static void ShakeLiveCamera(CameraShakeSettings settings)
    {
        liveCameraShake.Shake(settings);
    }

    public static void ZoomLiveCamera(CameraZoomSettings settings)
    {
        liveCameraZoom.Zoom(settings);
    }
}
