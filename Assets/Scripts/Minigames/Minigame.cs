using Events;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Minigame : MonoBehaviour
{
    public enum Type
    {
        None, Catcher, FallingFloors, Lanes, Ring,
    }

    protected abstract Type MinigameType { get; }

    [SerializeField] protected GameObject container;
    [SerializeField] protected int winScore = 1000;

    protected bool won;
    protected UnityAction onEnd;

    protected virtual void Start()
    {
        GameManager.EventService.Add<MinigameStartedEvent>(OnMinigameStarted);
        GameManager.EventService.Add<MinigameCancelledEvent>(OnMinigameCancelled);
        container.SetActive(false);
    }

    public virtual void OnMinigameStarted(MinigameStartedEvent evt)
    {
        if (evt.Type == MinigameType)
        {
            container.SetActive(true);
            won = false;
            onEnd = evt.OnEnd;
        }
    }

    protected virtual void OnMinigameCancelled()
    {
        if (GameManager.CurrentMinigame == MinigameType)
        {
            Debug.Log("[minigame] cancelling " + MinigameType);
            won = false;
            EndMinigame();
        }
    }

    protected virtual void EndMinigame()
    {
        onEnd?.Invoke();

        if (won)
        {
            GameManager.AddScore(winScore);
        }

        GameManager.EventService.Dispatch<MinigameEndedEvent>();
        container.SetActive(false);
    }

    protected void EndImmediate()
    {
        Notify();
        EndMinigame();
    }

    protected IEnumerator EndAfterDelay(float seconds = 2f)
    {
        Notify();
        yield return new WaitForSeconds(seconds);
        EndMinigame();
    }

    private void Notify()
    {
        NotificationManager.Notify(won ? "Minigame won!" : "Minigame lost!");
    }
}
