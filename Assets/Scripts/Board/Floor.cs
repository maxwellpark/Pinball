using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private AudioClip onEnterSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsBall())
        {
            onEnter?.Invoke();
            if (audioSource != null && onEnterSound)
            {
                audioSource.PlayOneShot(onEnterSound);
            }
        }
        // TODO: this means Floor can't be generic, and only used as a KillFloor (which it is currently)
        else if (collision.IsGhostBall())
        {
            Destroy(collision.gameObject, 0.25f);
        }
    }
}
