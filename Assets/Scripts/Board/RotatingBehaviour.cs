using UnityEngine;

public class RotatingBehaviour : ContinuousBehaviour
{
    [SerializeField] private float speed = 100f;

    protected override void Behave()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
