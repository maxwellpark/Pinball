using Events;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Minigame : MonoBehaviour
{
    [SerializeField] protected GameObject container;
    [SerializeField] protected int winScore = 1000;

    protected bool won;
    protected UnityAction onEnd;

    protected virtual void Start()
    {
        GameManager.EventService.Add<MinigameStartedEvent>(StartMinigame);
        container.SetActive(false);
    }

    public virtual void StartMinigame(MinigameStartedEvent evt)
    {
        container.SetActive(true);
        won = false;
        onEnd = evt.OnEnd;
    }

    protected virtual void EndMinigame()
    {
        onEnd?.Invoke();
        GameManager.EventService.Dispatch<MinigameEndedEvent>();
        container.SetActive(false);
    }

    protected IEnumerator EndAfterDelay()
    {
        NotificationManager.Notify(won ? "Minigame won!" : "Minigame lost!");
        yield return new WaitForSeconds(2f);
        EndMinigame();
    }
}
