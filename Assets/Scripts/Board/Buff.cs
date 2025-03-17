using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    [SerializeField] protected bool permanentlyActive;
    [SerializeField] AudioClip audioClip;
    private AudioSource audioSource;

    public bool IsActive { get; protected set; }
    protected virtual bool ShouldTrigger(Collider2D collision) => (permanentlyActive || IsActive) && collision.IsBall();
    protected abstract void TriggerBehaviour(Collider2D collision);

    protected virtual void Start()
    {
        if (TryGetComponent<BoxColliderDrawer>(out var drawer))
        {
            drawer.SetIsDrawing(() => permanentlyActive || IsActive);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Activate()
    {
        if (IsActive || permanentlyActive)
        {
            return;
        }

        NotificationManager.Notify($"{GetType()} activating...", 1f);
        IsActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ShouldTrigger(collision))
        {
            return;
        }

        Debug.Log($"{name} triggering effect...");
        TriggerBehaviour(collision);
        IsActive = false;

        // TODO: might want to make TriggerBehaviour virtual and stick the sfx stuff in there,
        // in case buffs aren't activated via collision. 
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
