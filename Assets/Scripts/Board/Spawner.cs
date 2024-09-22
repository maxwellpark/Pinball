using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private LayerMask mask;
    [SerializeField] private int maxAttempts = 10;

    public void Spawn()
    {
        var bounds = spawnArea.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            var position = new Vector2(x, y);

            if (!Physics2D.OverlapCircle(position, 0.5f, mask))
            {
                Instantiate(prefab, position, Quaternion.identity);
                break;
            }
        }
    }
}
