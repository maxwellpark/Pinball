using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BumperGroup : MonoBehaviour
{
    [SerializeField] private int scoreReceiverTrigger;
    private int currentScore;

    [SerializeField] private ReceiverBase receiver;
    [SerializeField] private bool isRepeatable;

    private List<Bumper> bumpers;

    public int CurrentScore => currentScore;
    public int ScoreReceiverTrigger => scoreReceiverTrigger;

    private void Start()
    {
        currentScore = 0;
        bumpers = GetComponentsInChildren<Bumper>().ToList();

        if (!bumpers.Any())
        {
            Debug.LogWarning($"[bumper group] {name} has no bumpers as children");
            Destroy(gameObject);
        }
        else
        {
            foreach (var bumper in bumpers)
            {
                bumper.OnScoreAdded += OnScoreAdded;
            }
        }

        UIManager.Instance.RegisterBumperGroup(this);
    }

    private void OnScoreAdded(int score)
    {
        Debug.Log($"[bumper group] {score} score added by child bumper");
        currentScore += Mathf.Max(0, score);
        UIManager.Instance.UpdateBumperGroupText(this);

        if (receiver != null && receiver.IsLocked && currentScore >= scoreReceiverTrigger)
        {
            Debug.Log($"[bumper group] unlocking {receiver.name} as {scoreReceiverTrigger} score reached");
            receiver.Unlock();

            if (isRepeatable)
            {
                currentScore = 0;
                return;
            }

            Debug.Log($"[bumper group] {name} destroying as non-repeatable");
            Unsubscribe();
            // Just destroy the component, not the gameObject that has the bumpers as children 
            Destroy(this);
            UIManager.Instance.UnregisterBumperGroup(this);
        }
    }

    private void Unsubscribe()
    {
        if (bumpers == null)
        {
            return;
        }

        foreach (var bumper in bumpers)
        {
            if (bumper != null)
            {
                bumper.OnScoreAdded -= OnScoreAdded;
            }
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
        UIManager.Instance.UnregisterBumperGroup(this);
    }
}
