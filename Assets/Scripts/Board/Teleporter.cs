using Cinemachine;
using UnityEngine;

public class Teleporter : ReceiverBase
{
    [SerializeField] private Transform destination;
    [SerializeField] private CinemachineVirtualCamera vcam;

    protected override void OnExit(Collider2D collision)
    {
        base.OnExit(collision);
        collision.transform.position = destination.position;

        if (vcam != null)
        {
            var ball = GameManager.Ball;
            if (ball == null)
            {
                return;
            }
            Debug.Log($"[port] vcam {vcam.name} following");
            vcam.Follow = ball.transform;
            CameraManager.SetPriority(vcam);
        }
    }
}
