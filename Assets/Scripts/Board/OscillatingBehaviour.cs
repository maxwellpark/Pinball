using UnityEngine;

public class OscillatingBehaviour : ContinuousBehaviour
{
    public float speed = 5f;
    public float distance = 3f;
    public bool startMovingRight = true; // Initial direction 

    private Vector3 startPos;
    private int direction; // 1 for right, -1 for left
    private float traveledDistance;

    private void Start()
    {
        startPos = transform.position;
        direction = startMovingRight ? 1 : -1;
        traveledDistance = 0f;
    }

    protected override void Behave()
    {
        var moveAmount = direction * speed * Time.deltaTime;
        transform.Translate(moveAmount, 0, 0);
        traveledDistance += Mathf.Abs(moveAmount);

        if (traveledDistance >= distance)
        {
            direction *= -1;
            traveledDistance = 0f;
        }
    }

    // TODO: might move up to base class 
    // Only referenced in inspector for now 
    public void Reset()
    {
        transform.position = startPos;
        direction = startMovingRight ? 1 : -1;
        traveledDistance = 0f;
    }
}
