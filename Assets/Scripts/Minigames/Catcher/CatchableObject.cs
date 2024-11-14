using Events;
using UnityEngine;

public class CatchableObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool collided;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collided)
        {
            return;
        }

        if (collision.gameObject.CompareTag(Tags.Catcher))
        {
            //Destroy(gameObject);
            spriteRenderer.color = Color.green;
            collided = true;
            GameManager.EventService.Dispatch<ObjectCaughtEvent>();
        }
        else if (collision.gameObject.CompareTag(Tags.Ground))
        {
            //Destroy(gameObject);
            spriteRenderer.color = Color.red;
            collided = true;
            GameManager.EventService.Dispatch<ObjectMissedEvent>();
        }
    }
}
