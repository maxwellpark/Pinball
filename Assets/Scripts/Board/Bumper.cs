using System.Collections;
using UnityEngine;

public class Bumper : BounceBehaviourBase
{
    [SerializeField] private float vibrationDuration = 0.2f;
    [SerializeField] private float vibrationIntensity = 0.1f;
    [SerializeField] private float vibrationSpeed = 40f;

    private Vector3 startPos;

    protected virtual void Start()
    {
        startPos = transform.localPosition;
    }

    protected override void OnCollision(Collider2D collider)
    {
        StartVibrate(vibrationDuration, vibrationIntensity, vibrationSpeed);
    }

    public void StartVibrate(float duration, float intensity, float speed)
    {
        StartCoroutine(Vibrate(duration, intensity, speed));
    }

    private IEnumerator Vibrate(float duration, float intensity, float speed)
    {
        var time = 0f;

        while (time < duration)
        {
            var xOffset = Mathf.Sin(Time.time * speed) * intensity;
            var yOffset = Mathf.Cos(Time.time * speed) * intensity;

            transform.localPosition = startPos + new Vector3(xOffset, yOffset, 0f);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}
