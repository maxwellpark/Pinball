public class CameraManager : Singleton<CameraManager>
{
    private static CinemachineLiveCameraShake liveCameraShake;

    protected override void Awake()
    {
        base.Awake();
        liveCameraShake = GetComponent<CinemachineLiveCameraShake>();
    }

    public static void ShakeLiveCamera(CameraShakeSettings settings)
    {
        liveCameraShake.Shake(settings);
    }
}
