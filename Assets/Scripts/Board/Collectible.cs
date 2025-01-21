using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private GameObject particlePrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsBallOrGhostBall())
        {
            return;
        }

        Debug.Log($"{collision.name} hit collectible {name}");
        GameManager.AddScore(score);
        Instantiate(particlePrefab, collision.transform.position, particlePrefab.transform.rotation);
        Destroy(gameObject);
    }
}
