using UnityEngine;

public class OscillatingBehaviour : ContinuousBehaviour
{
    public float speed = 5f;
    public float distance = 3f;
    public bool startMovingRight = true; // Initial direction 

    private Vector3 startPos;
    private float startX;
    private float endX;
    private int direction; // 1 for right, -1 for left

    // TODO: support vertical movement too?
    private void Start()
    {
        startPos = transform.position;
        startX = startPos.x;
        direction = startMovingRight ? 1 : -1;
        endX = (startX + distance) * direction;
    }

    protected override void Behave()
    {
        var moveAmount = direction * speed * Time.deltaTime;
        transform.Translate(moveAmount, 0, 0);

        if (direction == 1 && transform.position.x >= endX
            || direction == -1 && transform.position.x <= endX)
        {
            direction *= -1;
            startX = endX;
            endX = (startX + distance) * direction;
        }
    }

    // TODO: might move up to base class 
    // Only referenced in inspector for now 
    public void Reset()
    {
        transform.position = startPos;
        direction = startMovingRight ? 1 : -1;
    }
}
