using UnityEngine;
using UnityEngine.Events;

public class LanePlayer : MonoBehaviour
{
    [SerializeField] private Transform[] lanes;
    [SerializeField] private KeyCode[] upKeys = new[] { KeyCode.W, KeyCode.UpArrow };
    [SerializeField] private KeyCode[] downKeys = new[] { KeyCode.S, KeyCode.DownArrow };

    private int currentLaneIndex = 0;

    public event UnityAction OnHitGap;
    public event UnityAction OnFinished;

    private void OnEnable()
    {
        currentLaneIndex = 0;
        UpdatePosition();
    }

    private void Update()
    {
        if (Utils.AnyKeysDown(upKeys))
        {
            SwitchLane(-1);
        }
        else if (Utils.AnyKeysDown(downKeys))
        {
            SwitchLane(1);
        }
    }

    private void SwitchLane(int direction)
    {
        currentLaneIndex = Mathf.Clamp(currentLaneIndex + direction, 0, lanes.Length - 1);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, lanes[currentLaneIndex].position.y, transform.position.z);
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
