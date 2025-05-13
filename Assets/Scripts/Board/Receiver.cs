using System.Collections;
using UnityEngine;

// TODO: could this inherit from CollisionBehaviourBase? 
public abstract class ReceiverBase : MonoBehaviour
{
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private string animationName;
    [SerializeField] private float pullSpeed = 2f;
    [SerializeField] private bool startsLocked;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private AudioClip onEnterSound;
    //[SerializeField] private AudioClip onExitSound;

    private Color startingColor;
    protected bool isWaiting;
    private bool isLocked;
    public bool IsLocked
    {
        get => isLocked; set
        {
            isLocked = value;
            spriteRenderer.color = isLocked ? lockedColor : startingColor;
        }
    }
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        IsLocked = startsLocked;
    }

    protected virtual void OnEnter(Collider2D collision)
    {
        if (IsLocked || collision == null || collision.gameObject == null)
        {
            return;
        }

        if (collision.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
            ballRb.isKinematic = true;

            var trail = ballRb.GetComponent<TrailRenderer>();
            trail.Clear();
        }

        if (!string.IsNullOrEmpty(animationName))
        {
            var animator = collision.GetComponentInChildren<Animator>(true);
            animator.gameObject.SetActive(true);
            animator.Play(animationName);
        }

        if (audioSource != null && onEnterSound != null)
        {
            audioSource.PlayOneShot(onEnterSound);
        }
    }

    protected virtual void OnExit(Collider2D collision)
    {
        if (collision == null || collision.gameObject == null)
        {
            return;
        }

        if (collision.TryGetComponent<Rigidbody2D>(out var ballRb))
        {
            ballRb.isKinematic = false;
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        if (!string.IsNullOrEmpty(animationName))
        {
            var animator = collision.GetComponentInChildren<Animator>(true);
            animator.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsBall())
        {
            return;
        }

        StartCoroutine(Receive(collision));
    }

    private IEnumerator Receive(Collider2D collision)
    {
        OnEnter(collision);

        var ballTransform = collision.transform;
        var startPos = ballTransform.position;
        var endPos = transform.position;

        var timeElapsed = 0f;
        var distance = Vector2.Distance(startPos, endPos);

        while (distance > 0.01f)
        {
            timeElapsed += Time.deltaTime * pullSpeed;
            ballTransform.position = Vector2.Lerp(startPos, endPos, timeElapsed / distance);

            distance = Vector2.Distance(ballTransform.position, endPos);
            yield return null;
        }

        // 0 means just wait until told to stop 
        if (waitTime == 0)
        {
            isWaiting = true;

            while (isWaiting)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        OnExit(collision);
    }

    public void Unlock()
    {
        Debug.Log("[receiver] unlocking " + name);
        NotificationManager.Notify("Unlocking " + name);
        IsLocked = false;
    }
}
