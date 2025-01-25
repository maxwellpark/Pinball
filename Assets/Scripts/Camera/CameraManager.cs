using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CinemachineLiveCameraShake liveCameraShake;

    private void Awake()
    {
        liveCameraShake = GetComponent<CinemachineLiveCameraShake>();
    }

    public static void ShakeLiveCamera(CameraShakeSettings settings)
    {
        liveCameraShake.Shake(settings);
    }
}
