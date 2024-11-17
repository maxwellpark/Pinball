using UnityEngine;

public class SineWaveBehaviour : ContinuousBehaviour
{
    public float speed = 5f;
    public float amplitude = 2f;
    public float wavelength = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    protected override void Behave()
    {
        var newX = startPos.x + Time.time * speed;
        var newY = startPos.y + Mathf.Sin(newX / wavelength) * amplitude;

        transform.position = new Vector3(newX, newY, startPos.z);
    }
}
