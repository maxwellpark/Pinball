using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera ballCamera;
    [SerializeField] private CinemachineVirtualCamera flipperCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.IsBall(other))
        {
            ballCamera.Priority = 0;
            flipperCamera.Priority = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.IsBall(other))
        {
            ballCamera.Priority = 10;
            flipperCamera.Priority = 0;
        }
    }
}
