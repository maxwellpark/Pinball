using UnityEngine;

public class LanesMinigame : Minigame
{
    protected override Type MinigameType => Type.Lanes;

    [SerializeField] private LanePlayer player;

    protected override void Start()
    {
        base.Start();
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
        StartCoroutine(EndAfterDelay());
    }
}
