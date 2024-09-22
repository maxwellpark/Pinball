using System.Collections;
using UnityEngine;

public class Bumper : BounceBehaviourBase
{
    [SerializeField] private float vibrationDuration = 0.2f;
    [SerializeField] private float vibrationAmount = 0.1f;
    [SerializeField] private float vibrationSpeed = 40f;

    private Vector3 startPos;

    protected virtual void Start()
    {
        startPos = transform.localPosition;
    }

    protected override void OnCollision(Collider2D collider)
    {
        StartCoroutine(Vibrate());
    }

    private IEnumerator Vibrate()
    {
        var time = 0f;

        while (time < vibrationDuration)
        {
            var xOffset = Mathf.Sin(Time.time * vibrationSpeed) * vibrationAmount;
            var yOffset = Mathf.Cos(Time.time * vibrationSpeed) * vibrationAmount;

            transform.localPosition = startPos + new Vector3(xOffset, yOffset, 0f);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}
