using System;
using UnityEngine;

[Serializable]
public class CameraShakeSettings
{
    [Tooltip("The strength of the shake")]
    [field: SerializeField]
    public float Amplitude { get; private set; }

    [Tooltip("How long the shake lasts for")]
    [field: SerializeField]
    public float DurationInSeconds { get; private set; }

    public CameraShakeSettings() { }

    public CameraShakeSettings(float amplitude, float duration)
    {
        Amplitude = amplitude;
        DurationInSeconds = duration;
    }
}
