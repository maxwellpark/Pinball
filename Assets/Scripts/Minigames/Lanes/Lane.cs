using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void OnDisable()
    {
        transform.localPosition = startPos;
    }

    private void Update()
    {
        // For now need world space as we've rotated 90 degrees out of laziness 
        transform.Translate(speed * Time.deltaTime * Vector3.down, Space.World);

        // TODO: if making longer, make the lane respawn i.e. wrap around 
        // if (transform.position.y <= endPoint.position.y)
        // {
        //     transform.position = new Vector3(transform.position.x, respawnPoint.position.y, transform.position.z);
        // }
    }
}
