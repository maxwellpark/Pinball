using UnityEngine;

public class CameraShakeProducer : ProducerBase
{
    [SerializeField] private CameraShakeSettings shakeSettings;

    protected override void OnCollisionEnter2DInternal(Collision2D collision)
    {
        CameraManager.ShakeLiveCamera(shakeSettings);
    }

    protected override void OnDestroyInternal()
    {
        CameraManager.ShakeLiveCamera(shakeSettings);
    }
}
