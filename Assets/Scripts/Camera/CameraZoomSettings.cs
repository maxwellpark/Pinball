using System;
using UnityEngine;

[Serializable]
public class CameraZoomSettings
{
    [Tooltip("Orthographic lens size")]
    [field: SerializeField]
    public float TargetSize { get; private set; }

    [field: SerializeField]
    public float Speed { get; private set; }

    public CameraZoomSettings() { }

    public CameraZoomSettings(float targetSize, float speed)
    {
        TargetSize = targetSize;
        Speed = speed;
    }
}
