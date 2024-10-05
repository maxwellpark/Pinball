using UnityEngine;

public class LanePlayer : MonoBehaviour
{
    [SerializeField] private Transform[] lanes;
    private int currentLaneIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SwitchLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SwitchLane(1);
        }
    }

    private void SwitchLane(int direction)
    {
        currentLaneIndex = Mathf.Clamp(currentLaneIndex + direction, 0, lanes.Length - 1);
        transform.position = new Vector3(transform.position.x, lanes[currentLaneIndex].position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Gap))
        {
            Debug.Log("[lanes] player hit gap!");
        }
    }
}
