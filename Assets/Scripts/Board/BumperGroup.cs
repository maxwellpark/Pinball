using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BumperGroup : MonoBehaviour
{
    [SerializeField, Tooltip("The score accrued by hitting the child bumpers after which triggers the minigame")]
    private int scoreMinigameTrigger;
    private int currentScore;

    [SerializeField]
    private Minigame.Type minigameType;

    [SerializeField]
    private bool isRepeatable;

    private List<Bumper> bumpers;

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
    }

    private void OnScoreAdded(int score)
    {
        Debug.Log($"[bumper group] {score} score added by child bumper");
        currentScore += Mathf.Max(0, score);

        if (minigameType != Minigame.Type.None && currentScore >= scoreMinigameTrigger)
        {
            Debug.Log($"[bumper group] triggering {minigameType} minigame as score reached");
            GameManager.Instance.StartMinigame(minigameType, onStart: OnMinigameStart, onEnd: OnMinigameEnd);

            if (isRepeatable)
            {
                currentScore = 0;
                return;
            }

            Unsubscribe();
            // Just destroy the component, not the gameObject that has the bumpers as children 
            Destroy(this);
        }
    }

    // TODO: should handle this elsewhere but up until now it was all done via receivers...
    private void OnMinigameStart()
    {
        Debug.Log("[bumper group] OnMinigameStart");
        var ball = GameManager.Ball;
        if (ball == null)
        {
            Debug.LogWarning("[bumper group] ball was null when starting minigame");
            return;
        }

        ball.Freeze();
    }

    private void OnMinigameEnd()
    {
        Debug.Log("[bumper group] OnMinigameEnd");
        var ball = GameManager.Ball;
        if (ball == null)
        {
            Debug.LogWarning("[bumper group] ball was null when ending minigame");
            return;
        }

        ball.Unfreeze();
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
    }
}
