using Events;
using UnityEngine;

public class CatchableObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool caught;
    private bool missed;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (caught)
        {
            return;
        }

        if (collision.gameObject.CompareTag(Tags.Catcher))
        {
            //Destroy(gameObject);
            spriteRenderer.color = Color.green;
            caught = true;
            GameManager.EventService.Dispatch<ObjectCaughtEvent>();
        }
        else if (!missed && collision.gameObject.CompareTag(Tags.Ground))
        {
            //Destroy(gameObject);
            spriteRenderer.color = Color.red;
            missed = true;
            GameManager.EventService.Dispatch<ObjectMissedEvent>();
        }
    }
}
