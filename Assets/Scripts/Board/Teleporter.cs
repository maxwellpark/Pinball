using UnityEngine;

public class Teleporter : ReceiverBase
{
    [SerializeField] private Transform destination;

    protected override void OnExit(Collider2D collision)
    {
        base.OnExit(collision);
        collision.transform.position = destination.position;
    }
}
