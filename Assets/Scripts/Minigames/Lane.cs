using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform endPoint;

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.left);

        // TODO: if making longer, make the lane respawn i.e. wrap around 
        //if (transform.position.x >= endPoint.position.x)
        //{
        //    transform.position = new Vector3(respawnPoint.position.x, transform.position.y, transform.position.z);
        //}
    }
}
