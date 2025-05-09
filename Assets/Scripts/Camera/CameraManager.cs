using Cinemachine;
using Events;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        GameManager.EventService.Dispatch<CamerasUpdatedEvent>();
    }

    public static CinemachineVirtualCamera GetPriorityCamera()
    {
        // TODO: cache the priority cam if we end up with loads of them 
        return cameras.OrderByDescending(cam => cam.Priority).FirstOrDefault();
    }

    public static void SetPriority(CinemachineVirtualCamera camera, int priority = 10)
    {
        if (camera == null)
        {
            return;
        }
        Array.ForEach(cameras, c => c.Priority = 0);
        camera.Priority = priority;
        Debug.Log($"[camera] setting priority for {camera.Name} to {priority}");
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
