using UnityEngine;

public class LanesMinigame : Minigame
{
    protected override Type MinigameType => Type.Lanes;

    [SerializeField] private LanePlayer player;

    private void OnEnable()
    {
        player.OnHitGap += OnHitGap;
        player.OnFinished += OnFinished;
    }

    private void OnDisable()
    {
        player.OnHitGap -= OnHitGap;
        player.OnFinished -= OnFinished;
    }

    private void OnHitGap()
    {
        won = false;
        EndImmediate();
    }

    private void OnFinished()
    {
        won = true;
        EndImmediate();
    }
}
