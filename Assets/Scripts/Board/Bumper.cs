using System.Collections;
using UnityEngine;

public class Bumper : BounceBehaviourBase
{
    [Header("Vibration")]
    [SerializeField] protected float vibrationDuration = 0.2f;
    [SerializeField] protected float vibrationIntensity = 0.1f;
    [SerializeField] protected float vibrationSpeed = 40f;

    protected bool isVibrating;
    protected override bool IncludeGhostBalls => true;
    protected override bool UseOnCollisionEnter => true;
    protected override bool UseOnTriggerEnter => false;

    protected override void OnCollision(Collider2D collider)
    {
        base.OnCollision(collider);
        StartVibrate(vibrationDuration, vibrationIntensity, vibrationSpeed);
    }

    public void StartVibrate(float duration, float intensity, float speed)
    {
        StartCoroutine(Vibrate(duration, intensity, speed));
    }

    private IEnumerator Vibrate(float duration, float intensity, float speed)
    {
        Debug.Log("[bumper] start vibrating...");
        isVibrating = true;
        var startPos = transform.localPosition;
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
        isVibrating = false;
        Debug.Log("[bumper] stopped vibrating");
    }
}
