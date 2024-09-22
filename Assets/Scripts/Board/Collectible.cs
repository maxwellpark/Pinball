using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private GameObject particlePrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Utils.IsBall(collision))
        {
            return;
        }

        Debug.Log($"Hit collectible {name}");
        GameManager.AddScore(score);
        Instantiate(particlePrefab, collision.transform.position, particlePrefab.transform.rotation);
        Destroy(gameObject);
    }
}
