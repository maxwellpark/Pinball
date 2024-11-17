using UnityEngine;

public class OrbitingBehaviour : ContinuousBehaviour
{
    public Transform centerPoint;
    public float speed = 30f;
    public float radius = 2f;

    private float angle;

    private void Start()
    {
        if (centerPoint == null)
        {
            centerPoint = transform;
        }
    }

    protected override void Behave()
    {
        angle += speed * Time.deltaTime;

        var x = centerPoint.position.x + Mathf.Cos(angle) * radius;
        var y = centerPoint.position.y + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
