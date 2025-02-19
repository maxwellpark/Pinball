using Cinemachine;
using System.Collections;
using UnityEngine;

public class CinemachineLiveCameraShake : CinemachineLiveCameraBehaviour
{
    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private Coroutine shakeCoroutine;
    private float elapsedTime;
    private CameraShakeSettings currentSettings; // Cache this in case we need to restart the shake on cam changed 

    private void SetPerlinNoise()
    {
        if (virtualCamera != null)
        {
            perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (perlinNoise == null)
            {
                Debug.LogWarning(nameof(CinemachineBasicMultiChannelPerlin) + " component not found on " + nameof(CinemachineLiveCameraShake));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SetPerlinNoise();
    }

    protected override void OnCameraActivated(ICinemachineCamera active, ICinemachineCamera previous)
    {
        base.OnCameraActivated(active, previous);

        var midShake = shakeCoroutine != null;
        if (midShake)
        {
            StopShake();
        }

        SetPerlinNoise();

        if (midShake)
        {
            if (currentSettings == null)
            {
                Debug.LogWarning("[camera] unexpected null CameraShakeSettings");
                return;
            }
            // Restart the shake on the new live cam (elapsed time persists over) 
            Shake(currentSettings.Amplitude, currentSettings.DurationInSeconds);
        }
    }

    /// <summary>
    /// Shake over a duration.
    /// </summary>
    private void Shake(float amplitude, float duration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        Debug.Log("[camera] starting shake");
        shakeCoroutine = StartCoroutine(ShakeRoutine(amplitude, duration));
    }

    public void Shake(CameraShakeSettings settings)
    {
        // TODO: this is redundant now we listen for CameraActivated event in base class? 
        //UpdateActiveCamera();
        Shake(settings.Amplitude, settings.DurationInSeconds);
        currentSettings = settings;
    }

    public void StopShake()
    {
        if (shakeCoroutine == null)
        {
            return;
        }

        Debug.Log("[camera] stopping shake");
        StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;
        perlinNoise.m_AmplitudeGain = 0f;
    }

    private IEnumerator ShakeRoutine(float amplitude, float duration)
    {
        if (perlinNoise == null)
        {
            yield break;
        }
        perlinNoise.m_AmplitudeGain = amplitude;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        perlinNoise.m_AmplitudeGain = 0f;
        shakeCoroutine = null;
        currentSettings = null;
        Debug.Log("[camera] shake routine ran to completion");
        elapsedTime = 0f;
    }
}
