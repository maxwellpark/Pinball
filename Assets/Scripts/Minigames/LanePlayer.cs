using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LanePlayer : MonoBehaviour
{
    [SerializeField] private Transform[] lanes;
    [SerializeField] private KeyCode[] leftKeys = new[] { KeyCode.A, KeyCode.LeftArrow };
    [SerializeField] private KeyCode[] rightKeys = new[] { KeyCode.D, KeyCode.RightArrow };
    [SerializeField] private float switchEffectDuration = 0.2f;
    [SerializeField] private float switchScaleAmount = 1.2f;

    private int currentLaneIndex = 0;
    private Coroutine switchEffectRoutine;

    public event UnityAction OnHitGap;
    public event UnityAction OnFinished;

    private void OnEnable()
    {
        currentLaneIndex = 0;
        UpdatePosition();
    }

    private void Update()
    {
        if (Utils.AnyKeysDown(leftKeys))
        {
            SwitchLane(-1);
        }
        else if (Utils.AnyKeysDown(rightKeys))
        {
            SwitchLane(1);
        }
    }

    private void SwitchLane(int direction)
    {
        currentLaneIndex = Mathf.Clamp(currentLaneIndex + direction, 0, lanes.Length - 1);

        if (switchEffectRoutine != null)
        {
            StopCoroutine(switchEffectRoutine);
        }
        switchEffectRoutine = StartCoroutine(SwitchLaneEffect(lanes[currentLaneIndex]));

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(lanes[currentLaneIndex].position.x, transform.position.y, transform.position.z);
    }

    private IEnumerator SwitchLaneEffect(Transform lane)
    {
        var originalScale = lane.localScale;
        var targetScale = originalScale * switchScaleAmount;

        var elapsedTime = 0f;
        while (elapsedTime < switchEffectDuration)
        {
            lane.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / switchEffectDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lane.localScale = originalScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Gap))
        {
            Debug.Log("[lanes] player hit gap!");
            OnHitGap?.Invoke();
        }

        if (other.CompareTag(Tags.Finish))
        {
            Debug.Log("[lanes] player reached finish!");
            OnFinished?.Invoke();
        }
    }
}
